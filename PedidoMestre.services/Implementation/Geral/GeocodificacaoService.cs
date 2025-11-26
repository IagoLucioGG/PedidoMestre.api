using System.Text.Json;
using Microsoft.Extensions.Logging;
using PedidoMestre.Models.Common;
using PedidoMestre.Services.Infrastructure;
using PedidoMestre.Services.Interfaces;
using Polly;
using Polly.Retry;

namespace PedidoMestre.Services.Implementation.Geral
{
    public class GeocodificacaoService : IGeocodificacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeocodificacaoService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public GeocodificacaoService(HttpClient httpClient, ILogger<GeocodificacaoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _retryPolicy = HttpRetryPolicy.CreateRetryPolicy(maxRetries: 3, logger);
            
            // Configurar User-Agent (obrigatório para Nominatim)
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PedidoMestre/1.0");
        }

        public async Task<ResponseModel<Coordenadas>> ObterCoordenadasPorEnderecoAsync(string endereco, string cidade, string? cep = null)
        {
            try
            {
                // Montar endereço completo para busca
                var enderecoCompleto = $"{endereco}, {cidade}, Brasil";
                if (!string.IsNullOrEmpty(cep))
                {
                    enderecoCompleto = $"{endereco}, {cidade}, {cep}, Brasil";
                }

                // URL da API Nominatim
                var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(enderecoCompleto)}&format=json&limit=1&addressdetails=1";

                _logger.LogInformation("Buscando coordenadas para: {Endereco}", enderecoCompleto);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url)
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Erro ao buscar coordenadas: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var resultados = JsonSerializer.Deserialize<JsonElement[]>(jsonResponse);

                if (resultados == null || resultados.Length == 0)
                {
                    return new ResponseModel<Coordenadas>("Endereço não encontrado. Verifique se o endereço está correto.");
                }

                var primeiroResultado = resultados[0];
                var lat = primeiroResultado.GetProperty("lat").GetString();
                var lon = primeiroResultado.GetProperty("lon").GetString();
                var displayName = primeiroResultado.TryGetProperty("display_name", out var displayNameProp) 
                    ? displayNameProp.GetString() 
                    : null;

                if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lon))
                {
                    return new ResponseModel<Coordenadas>("Coordenadas não encontradas para o endereço informado.");
                }

                var coordenadas = new Coordenadas
                {
                    Latitude = decimal.Parse(lat),
                    Longitude = decimal.Parse(lon),
                    EnderecoCompleto = displayName
                };

                // Rate limiting: Nominatim pede para aguardar 1 segundo entre requisições
                await Task.Delay(1000);

                return new ResponseModel<Coordenadas>(coordenadas, "Coordenadas obtidas com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter coordenadas para endereço: {Endereco}", endereco);
                return new ResponseModel<Coordenadas>($"Erro ao obter coordenadas: {ex.Message}");
            }
        }

        public async Task<ResponseModel<Coordenadas>> ObterCoordenadasPorCepAsync(string cep)
        {
            try
            {
                // Primeiro, buscar endereço completo pelo CEP usando ViaCEP
                var viaCepUrl = $"https://viacep.com.br/ws/{cep.Replace("-", "").Replace(".", "")}/json/";
                
                _logger.LogInformation("Buscando endereço pelo CEP: {Cep}", cep);

                var viaCepResponse = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(viaCepUrl)
                );
                
                if (!viaCepResponse.IsSuccessStatusCode)
                {
                    return new ResponseModel<Coordenadas>("CEP não encontrado.");
                }

                var viaCepJson = await viaCepResponse.Content.ReadAsStringAsync();
                var enderecoViaCep = JsonSerializer.Deserialize<JsonElement>(viaCepJson);

                if (enderecoViaCep.TryGetProperty("erro", out _))
                {
                    return new ResponseModel<Coordenadas>("CEP inválido ou não encontrado.");
                }

                var logradouro = enderecoViaCep.GetProperty("logradouro").GetString() ?? "";
                var bairro = enderecoViaCep.GetProperty("bairro").GetString() ?? "";
                var localidade = enderecoViaCep.GetProperty("localidade").GetString() ?? "";
                var uf = enderecoViaCep.GetProperty("uf").GetString() ?? "";

                // Montar endereço completo
                var enderecoCompleto = $"{logradouro}, {bairro}, {localidade}, {uf}, Brasil";

                // Agora buscar coordenadas usando Nominatim
                return await ObterCoordenadasPorEnderecoAsync(enderecoCompleto, localidade, cep);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter coordenadas pelo CEP: {Cep}", cep);
                return new ResponseModel<Coordenadas>($"Erro ao obter coordenadas pelo CEP: {ex.Message}");
            }
        }

        public async Task<ResponseModel<EnderecoCompleto>> ObterEnderecoCompletoAsync(string endereco)
        {
            try
            {
                // Montar endereço completo para busca (adicionar Brasil para melhorar resultados)
                var enderecoCompleto = $"{endereco}, Brasil";

                // URL da API Nominatim com addressdetails para obter cidade e estado
                var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(enderecoCompleto)}&format=json&limit=1&addressdetails=1";

                _logger.LogInformation("Buscando endereço completo para: {Endereco}", enderecoCompleto);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url)
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Erro ao buscar endereço: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var resultados = JsonSerializer.Deserialize<JsonElement[]>(jsonResponse);

                if (resultados == null || resultados.Length == 0)
                {
                    return new ResponseModel<EnderecoCompleto>("Endereço não encontrado. Verifique se o endereço está correto.");
                }

                var primeiroResultado = resultados[0];
                var lat = primeiroResultado.GetProperty("lat").GetString();
                var lon = primeiroResultado.GetProperty("lon").GetString();
                var displayName = primeiroResultado.TryGetProperty("display_name", out var displayNameProp) 
                    ? displayNameProp.GetString() 
                    : null;

                if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lon))
                {
                    return new ResponseModel<EnderecoCompleto>("Coordenadas não encontradas para o endereço informado.");
                }

                // Extrair cidade e UF do addressdetails
                string? cidade = null;
                string? uf = null;

                if (primeiroResultado.TryGetProperty("address", out var address))
                {
                    // Tentar diferentes campos para cidade
                    if (address.TryGetProperty("city", out var cityProp))
                        cidade = cityProp.GetString();
                    else if (address.TryGetProperty("town", out var townProp))
                        cidade = townProp.GetString();
                    else if (address.TryGetProperty("municipality", out var municipalityProp))
                        cidade = municipalityProp.GetString();
                    else if (address.TryGetProperty("village", out var villageProp))
                        cidade = villageProp.GetString();

                    // Tentar diferentes campos para estado/UF
                    if (address.TryGetProperty("state", out var stateProp))
                    {
                        var state = stateProp.GetString();
                        // Extrair sigla do estado (ex: "São Paulo" -> "SP")
                        uf = ExtrairUfDoEstado(state);
                    }
                    else if (address.TryGetProperty("state_code", out var stateCodeProp))
                    {
                        uf = stateCodeProp.GetString();
                    }
                }

                var enderecoCompletoObj = new EnderecoCompleto
                {
                    Latitude = decimal.Parse(lat),
                    Longitude = decimal.Parse(lon),
                    Cidade = cidade,
                    Uf = uf,
                    EnderecoFormatado = displayName
                };

                // Rate limiting: Nominatim pede para aguardar 1 segundo entre requisições
                await Task.Delay(1000);

                return new ResponseModel<EnderecoCompleto>(enderecoCompletoObj, "Endereço completo obtido com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter endereço completo para: {Endereco}", endereco);
                return new ResponseModel<EnderecoCompleto>($"Erro ao obter endereço completo: {ex.Message}");
            }
        }

        private string? ExtrairUfDoEstado(string? estado)
        {
            if (string.IsNullOrEmpty(estado))
                return null;

            // Mapeamento básico de estados brasileiros para UF
            var estadosUf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Acre", "AC" }, { "Alagoas", "AL" }, { "Amapá", "AP" }, { "Amazonas", "AM" },
                { "Bahia", "BA" }, { "Ceará", "CE" }, { "Distrito Federal", "DF" },
                { "Espírito Santo", "ES" }, { "Goiás", "GO" }, { "Maranhão", "MA" },
                { "Mato Grosso", "MT" }, { "Mato Grosso do Sul", "MS" }, { "Minas Gerais", "MG" },
                { "Pará", "PA" }, { "Paraíba", "PB" }, { "Paraná", "PR" }, { "Pernambuco", "PE" },
                { "Piauí", "PI" }, { "Rio de Janeiro", "RJ" }, { "Rio Grande do Norte", "RN" },
                { "Rio Grande do Sul", "RS" }, { "Rondônia", "RO" }, { "Roraima", "RR" },
                { "Santa Catarina", "SC" }, { "São Paulo", "SP" }, { "Sergipe", "SE" },
                { "Tocantins", "TO" }
            };

            // Se já for uma sigla de 2 letras, retornar
            if (estado.Length == 2)
                return estado.ToUpper();

            // Buscar no dicionário
            if (estadosUf.TryGetValue(estado, out var uf))
                return uf;

            // Tentar extrair sigla do nome (ex: "São Paulo" -> "SP")
            var palavras = estado.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (palavras.Length >= 2)
            {
                var sigla = palavras[0][0].ToString().ToUpper() + palavras[1][0].ToString().ToUpper();
                return sigla;
            }

            return null;
        }
    }
}

