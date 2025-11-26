using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Geral;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Services.Infrastructure;
using PedidoMestre.Services.Interfaces;
using Polly;
using Polly.Retry;

namespace PedidoMestre.Services.Implementation.Geral
{
    public class BairroService : IBairroService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BairroService> _logger;
        private readonly IGeocodificacaoService _geocodificacaoService;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public BairroService(
            AppDbContext context,
            HttpClient httpClient,
            ILogger<BairroService> logger,
            IGeocodificacaoService geocodificacaoService)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
            _geocodificacaoService = geocodificacaoService;
            _retryPolicy = HttpRetryPolicy.CreateRetryPolicy(maxRetries: 3, logger);
        }

        public async Task<ResponseModel<IEnumerable<Bairro>>> BuscarBairrosPorCidadeAsync(string cidade, string uf)
        {
            try
            {
                // 1. Buscar código IBGE da cidade
                var codigoIbge = await ObterCodigoIbgeCidadeAsync(cidade, uf);
                
                if (string.IsNullOrEmpty(codigoIbge))
                {
                    return new ResponseModel<IEnumerable<Bairro>>($"Cidade '{cidade}' não encontrada no estado '{uf}'");
                }

                // 2. Buscar bairros usando API do IBGE ou alternativa
                var bairrosInfo = await ObterBairrosPorCodigoIbgeAsync(codigoIbge, cidade);

                // Converter BairroInfo para Bairro (sem IdLoja ainda, será definido ao criar)
                var bairros = bairrosInfo.Select(bi => new Bairro
                {
                    Nome = bi.Nome,
                    TaxaEntrega = 0, // Será calculado ao criar
                    Latitude = bi.Latitude,
                    Longitude = bi.Longitude
                }).ToList();

                return new ResponseModel<IEnumerable<Bairro>>(bairros, $"Bairros obtidos com sucesso para {cidade}/{uf}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar bairros para cidade: {Cidade}/{Uf}", cidade, uf);
                return new ResponseModel<IEnumerable<Bairro>>($"Erro ao buscar bairros: {ex.Message}");
            }
        }

        public async Task<ResponseModel<IEnumerable<Bairro>>> CriarBairrosAutomaticamenteAsync(
            int idLoja, 
            string cidade, 
            string uf, 
            decimal? latitudeLoja = null, 
            decimal? longitudeLoja = null,
            decimal? taxaPorKm = null)
        {
            try
            {
                // Verificar se a loja existe
                var loja = await _context.Lojas
                    .Include(l => l.Bairros)
                    .FirstOrDefaultAsync(l => l.IdLoja == idLoja);

                if (loja == null)
                {
                    throw new KeyNotFoundException($"Loja com ID {idLoja} não encontrada");
                }

                // Se não tiver coordenadas da loja, buscar pelo endereço
                if (!latitudeLoja.HasValue || !longitudeLoja.HasValue)
                {
                    var coordenadasLoja = await _geocodificacaoService.ObterCoordenadasPorEnderecoAsync(
                        loja.Endereco, 
                        cidade
                    );

                    if (coordenadasLoja.Status && coordenadasLoja.Dados != null)
                    {
                        latitudeLoja = coordenadasLoja.Dados.Latitude;
                        longitudeLoja = coordenadasLoja.Dados.Longitude;

                        // Atualizar coordenadas da loja
                        loja.Latitude = latitudeLoja;
                        loja.Longitude = longitudeLoja;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new InvalidOperationException("Não foi possível obter coordenadas da loja para calcular distâncias");
                    }
                }

                // Buscar bairros da cidade
                var resultadoBairros = await BuscarBairrosPorCidadeAsync(cidade, uf);
                
                if (!resultadoBairros.Status || resultadoBairros.Dados == null)
                {
                    return new ResponseModel<IEnumerable<Bairro>>(resultadoBairros.Mensagem);
                }

                var bairrosEncontrados = resultadoBairros.Dados.ToList();
                var bairrosCriados = new List<Bairro>();

                // Para cada bairro, calcular distância e taxa
                foreach (var bairroInfo in bairrosEncontrados)
                {
                    // Verificar se o bairro já existe para esta loja
                    var bairroExistente = await _context.Bairros
                        .FirstOrDefaultAsync(b => b.IdLoja == idLoja && b.Nome == bairroInfo.Nome);

                    if (bairroExistente != null)
                    {
                        continue; // Pula se já existe
                    }

                    // Calcular distância da loja até o bairro (usar coordenadas do centro do bairro se disponível)
                    decimal distanciaKm = 0;
                    decimal taxaEntrega = 5.00m; // Taxa mínima padrão
                    
                    // Usar taxa por KM informada ou valor padrão de R$ 7,50
                    decimal valorTaxaPorKm = taxaPorKm ?? 7.50m;

                    if (bairroInfo.Latitude.HasValue && bairroInfo.Longitude.HasValue)
                    {
                        distanciaKm = (decimal)CalcularDistancia(
                            latitudeLoja.Value,
                            longitudeLoja.Value,
                            bairroInfo.Latitude.Value,
                            bairroInfo.Longitude.Value
                        );

                        // Calcular taxa baseada na distância
                        // Fórmula: Taxa mínima (R$ 5,00) + (distância * taxa por KM)
                        taxaEntrega = 5.00m + (distanciaKm * valorTaxaPorKm);
                        
                        // Limitar taxa máxima (ex: R$ 30,00)
                        if (taxaEntrega > 30.00m)
                        {
                            taxaEntrega = 30.00m;
                        }
                    }

                    // Criar bairro
                    var novoBairro = new Bairro
                    {
                        IdLoja = idLoja,
                        Nome = bairroInfo.Nome,
                        TaxaEntrega = taxaEntrega,
                        Latitude = bairroInfo.Latitude,
                        Longitude = bairroInfo.Longitude
                    };

                    _context.Bairros.Add(novoBairro);
                    bairrosCriados.Add(novoBairro);
                }

                await _context.SaveChangesAsync();

                return new ResponseModel<IEnumerable<Bairro>>(
                    bairrosCriados, 
                    $"Foram criados {bairrosCriados.Count} bairros automaticamente para a loja"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar bairros automaticamente para loja {IdLoja}", idLoja);
                return new ResponseModel<IEnumerable<Bairro>>($"Erro ao criar bairros: {ex.Message}");
            }
        }

        public async Task<ResponseModel<Bairro>> AtualizarTaxaAsync(int idBairro, decimal novaTaxa)
        {
            var bairro = await _context.Bairros.FindAsync(idBairro);

            if (bairro == null)
            {
                throw new KeyNotFoundException($"Bairro com ID {idBairro} não encontrado");
            }

            bairro.TaxaEntrega = novaTaxa;
            await _context.SaveChangesAsync();

            return new ResponseModel<Bairro>(bairro, "Taxa atualizada com sucesso");
        }

        private async Task<string?> ObterCodigoIbgeCidadeAsync(string cidade, string uf)
        {
            try
            {
                // Usar API do IBGE para buscar código da cidade
                var url = $"https://servicodados.ibge.gov.br/api/v1/localidades/estados/{uf}/municipios";
                
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url)
                );
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var municipios = JsonSerializer.Deserialize<JsonElement[]>(json);

                if (municipios == null)
                {
                    return null;
                }

                // Buscar cidade (case insensitive)
                var municipio = municipios.FirstOrDefault(m =>
                    m.GetProperty("nome").GetString()?.Equals(cidade, StringComparison.OrdinalIgnoreCase) == true
                );

                if (municipio.ValueKind == JsonValueKind.Undefined)
                {
                    return null;
                }

                return municipio.GetProperty("id").GetString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar código IBGE para cidade: {Cidade}/{Uf}", cidade, uf);
                return null;
            }
        }

        private async Task<List<BairroInfo>> ObterBairrosPorCodigoIbgeAsync(string codigoIbge, string cidade)
        {
            var bairros = new List<BairroInfo>();

            try
            {
                // Tentar usar API do IBGE para distritos (que podem incluir bairros)
                // Nota: IBGE não tem endpoint direto de bairros, então vamos usar uma abordagem alternativa
                
                // Opção 1: Usar API de localidades do IBGE (distritos)
                var urlDistritos = $"https://servicodados.ibge.gov.br/api/v1/localidades/municipios/{codigoIbge}/distritos";
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(urlDistritos)
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var distritos = JsonSerializer.Deserialize<JsonElement[]>(json);

                    if (distritos != null)
                    {
                        foreach (var distrito in distritos)
                        {
                            var nomeDistrito = distrito.GetProperty("nome").GetString();
                            if (!string.IsNullOrEmpty(nomeDistrito))
                            {
                                // Tentar obter coordenadas do distrito/bairro
                                var coordenadas = await _geocodificacaoService.ObterCoordenadasPorEnderecoAsync(
                                    nomeDistrito,
                                    cidade
                                );

                                bairros.Add(new BairroInfo
                                {
                                    Nome = nomeDistrito,
                                    Latitude = coordenadas.Status && coordenadas.Dados != null 
                                        ? coordenadas.Dados.Latitude 
                                        : null,
                                    Longitude = coordenadas.Status && coordenadas.Dados != null 
                                        ? coordenadas.Dados.Longitude 
                                        : null
                                });

                                // Rate limiting
                                await Task.Delay(1000);
                            }
                        }
                    }
                }

                // Se não encontrou distritos ou lista está vazia, usar lista comum de bairros brasileiros
                // (pode ser expandida com uma base de dados local ou outra API)
                if (bairros.Count == 0)
                {
                    // Retornar lista vazia - o usuário pode cadastrar manualmente
                    _logger.LogWarning("Nenhum bairro encontrado para cidade {Cidade} via IBGE", cidade);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar bairros por código IBGE: {CodigoIbge}", codigoIbge);
            }

            return bairros;
        }

        private double CalcularDistancia(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double raioTerraKm = 6371.0;

            var dLat = GrausParaRadianos((double)(lat2 - lat1));
            var dLon = GrausParaRadianos((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(GrausParaRadianos((double)lat1)) *
                    Math.Cos(GrausParaRadianos((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return raioTerraKm * c;
        }

        private double GrausParaRadianos(double graus)
        {
            return graus * (Math.PI / 180.0);
        }

        private class BairroInfo
        {
            public string Nome { get; set; } = string.Empty;
            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }
        }
    }
}

