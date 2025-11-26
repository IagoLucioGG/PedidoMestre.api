using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Geral;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Geral
{
    public class TaxaEntregaService : ITaxaEntregaService
    {
        private readonly AppDbContext _context;

        public TaxaEntregaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<CalculoTaxaEntregaResultado>> CalcularTaxaEntregaAsync(long idEndereco, int? idLojaPreferida = null)
        {
            // 1. Buscar endereço com bairro
            var endereco = await _context.Enderecos
                .Include(e => e.Bairro)
                    .ThenInclude(b => b.Loja)
                .FirstOrDefaultAsync(e => e.IdEndereco == idEndereco);

            if (endereco == null)
            {
                throw new KeyNotFoundException($"Endereço com ID {idEndereco} não encontrado");
            }

            if (endereco.Bairro == null)
            {
                throw new ArgumentException("Endereço não possui bairro vinculado");
            }

            // 2. Buscar todas as lojas que atendem o bairro
            var lojasDoBairro = await _context.Bairros
                .Include(b => b.Loja)
                .Where(b => b.Nome == endereco.Bairro.Nome || b.IdLoja == endereco.Bairro.IdLoja)
                .Select(b => new
                {
                    Loja = b.Loja,
                    TaxaEntrega = b.TaxaEntrega,
                    BairroId = b.IdBairro
                })
                .ToListAsync();

            if (!lojasDoBairro.Any())
            {
                throw new InvalidOperationException($"Nenhuma loja encontrada para o bairro {endereco.Bairro.Nome}");
            }

            // 3. Filtrar apenas lojas abertas
            var lojasAbertas = lojasDoBairro
                .Where(l => l.Loja.Status.ToLower() == "aberta" || l.Loja.Status.ToLower() == "aberto")
                .ToList();

            // 4. Se nenhuma loja aberta, verificar se há loja preferida e se está fechada
            if (!lojasAbertas.Any())
            {
                var todasLojas = lojasDoBairro.Select(l => l.Loja).ToList();
                if (idLojaPreferida.HasValue && todasLojas.Any(l => l.IdLoja == idLojaPreferida.Value))
                {
                    throw new InvalidOperationException($"A loja selecionada está fechada no momento. Nenhuma loja disponível para entrega.");
                }
                throw new InvalidOperationException("Nenhuma loja disponível para entrega no momento. Todas as lojas estão fechadas.");
            }

            // 5. Calcular distâncias se tiver coordenadas
            var lojasComDistancia = lojasAbertas.Select(l => new
            {
                Loja = l.Loja,
                TaxaEntrega = l.TaxaEntrega,
                DistanciaKm = CalcularDistancia(
                    endereco.Latitude, 
                    endereco.Longitude,
                    l.Loja.Latitude,
                    l.Loja.Longitude
                )
            }).ToList();

            // 6. Verificar raio de entrega
            var lojasDentroDoRaio = lojasComDistancia
                .Where(l => !l.Loja.RaioEntrega.HasValue || l.DistanciaKm <= (double)l.Loja.RaioEntrega.Value)
                .OrderBy(l => l.DistanciaKm)
                .ToList();

            if (!lojasDentroDoRaio.Any())
            {
                throw new InvalidOperationException("Nenhuma loja dentro do raio de entrega disponível para este endereço.");
            }

            // 7. Selecionar loja (preferida ou mais próxima)
            var lojaSelecionada = idLojaPreferida.HasValue
                ? lojasDentroDoRaio.FirstOrDefault(l => l.Loja.IdLoja == idLojaPreferida.Value)
                : lojasDentroDoRaio.First();

            if (lojaSelecionada == null && idLojaPreferida.HasValue)
            {
                // Loja preferida não está dentro do raio, usar a mais próxima
                lojaSelecionada = lojasDentroDoRaio.First();
            }

            if (lojaSelecionada == null)
            {
                throw new InvalidOperationException("Não foi possível selecionar uma loja para entrega.");
            }

            var lojaMaisProxima = lojasDentroDoRaio.First();
            var isLojaMaisProxima = lojaSelecionada.Loja.IdLoja == lojaMaisProxima.Loja.IdLoja;

            // 8. Calcular taxa final
            var taxaFinal = lojaSelecionada.TaxaEntrega;

            if (lojaSelecionada == null)
            {
                throw new InvalidOperationException("Não foi possível selecionar uma loja para entrega.");
            }

            // Se não for a loja mais próxima, pode aplicar taxa adicional
            if (!isLojaMaisProxima && lojaSelecionada.DistanciaKm > lojaMaisProxima.DistanciaKm)
            {
                var diferencaKm = lojaSelecionada.DistanciaKm - lojaMaisProxima.DistanciaKm;
                // Taxa adicional de 1 real por km adicional (pode ser configurável)
                var taxaAdicional = (decimal)(diferencaKm * 1.0);
                taxaFinal += taxaAdicional;
            }

            var resultado = new CalculoTaxaEntregaResultado
            {
                IdLoja = lojaSelecionada.Loja.IdLoja,
                NomeLoja = lojaSelecionada.Loja.Endereco, // Pode ter um campo Nome na Loja
                TaxaEntrega = taxaFinal,
                DistanciaKm = (decimal)lojaSelecionada.DistanciaKm,
                LojaMaisProxima = isLojaMaisProxima,
                Observacao = !isLojaMaisProxima 
                    ? $"Loja mais próxima está fechada. Taxa calculada para loja alternativa (distância adicional: {(lojaSelecionada.DistanciaKm - lojaMaisProxima.DistanciaKm):F2} km)"
                    : null
            };

            return new ResponseModel<CalculoTaxaEntregaResultado>(resultado, "Taxa de entrega calculada com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<LojaDisponivel>>> ObterLojasDisponiveisAsync(long idEndereco)
        {
            // 1. Buscar endereço com bairro
            var endereco = await _context.Enderecos
                .Include(e => e.Bairro)
                    .ThenInclude(b => b.Loja)
                .FirstOrDefaultAsync(e => e.IdEndereco == idEndereco);

            if (endereco == null)
            {
                throw new KeyNotFoundException($"Endereço com ID {idEndereco} não encontrado");
            }

            if (endereco.Bairro == null)
            {
                throw new ArgumentException("Endereço não possui bairro vinculado");
            }

            // 2. Buscar todas as lojas que atendem o bairro
            var lojasDoBairro = await _context.Bairros
                .Include(b => b.Loja)
                .Where(b => b.Nome == endereco.Bairro.Nome || b.IdLoja == endereco.Bairro.IdLoja)
                .Select(b => new
                {
                    Loja = b.Loja,
                    TaxaEntrega = b.TaxaEntrega,
                    BairroId = b.IdBairro
                })
                .ToListAsync();

            // 3. Calcular distâncias e preparar resultado
            var lojasDisponiveis = lojasDoBairro.Select(l => new LojaDisponivel
            {
                IdLoja = l.Loja.IdLoja,
                NomeLoja = l.Loja.Endereco, // Pode ter um campo Nome na Loja
                TaxaEntrega = l.TaxaEntrega,
                DistanciaKm = (decimal)CalcularDistancia(
                    endereco.Latitude,
                    endereco.Longitude,
                    l.Loja.Latitude,
                    l.Loja.Longitude
                ),
                Status = l.Loja.Status
            })
            .OrderBy(l => l.DistanciaKm)
            .ThenBy(l => l.Status == "aberta" || l.Status == "aberto" ? 0 : 1)
            .ToList();

            return new ResponseModel<IEnumerable<LojaDisponivel>>(lojasDisponiveis, "Lojas disponíveis obtidas com sucesso");
        }

        /// <summary>
        /// Calcula a distância em quilômetros entre duas coordenadas usando a fórmula de Haversine
        /// </summary>
        private double CalcularDistancia(decimal? lat1, decimal? lon1, decimal? lat2, decimal? lon2)
        {
            // Se não tiver coordenadas, retorna 0 (não calcula distância)
            if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
            {
                return 0;
            }

            const double raioTerraKm = 6371.0;

            var dLat = GrausParaRadianos((double)(lat2.Value - lat1.Value));
            var dLon = GrausParaRadianos((double)(lon2.Value - lon1.Value));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(GrausParaRadianos((double)lat1.Value)) *
                    Math.Cos(GrausParaRadianos((double)lat2.Value)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return raioTerraKm * c;
        }

        private double GrausParaRadianos(double graus)
        {
            return graus * (Math.PI / 180.0);
        }
    }
}

