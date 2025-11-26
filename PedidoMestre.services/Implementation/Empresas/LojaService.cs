using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Empresas
{
    public class LojaService : ILojaService
    {
        private readonly AppDbContext _context;
        private readonly IBairroService _bairroService;
        private readonly IGeocodificacaoService _geocodificacaoService;

        public LojaService(
            AppDbContext context, 
            IBairroService bairroService,
            IGeocodificacaoService geocodificacaoService)
        {
            _context = context;
            _bairroService = bairroService;
            _geocodificacaoService = geocodificacaoService;
        }

        public async Task<ResponseModel<IEnumerable<Loja>>> ObterTodosAsync()
        {
            var lojas = await _context.Lojas
                .Include(l => l.Empresa)
                .Select(l => new Loja
                {
                    IdLoja = l.IdLoja,
                    IdEmpresa = l.IdEmpresa,
                    Endereco = l.Endereco,
                    Telefone = l.Telefone,
                    Status = l.Status,
                    ConfigDelivery = l.ConfigDelivery,
                    Empresa = l.Empresa
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Loja>>(lojas, "Lojas obtidas com sucesso");
        }

        public async Task<ResponseModel<Loja>> ObterPorIdAsync(int id)
        {
            var loja = await _context.Lojas
                .Include(l => l.Empresa)
                .Select(l => new Loja
                {
                    IdLoja = l.IdLoja,
                    IdEmpresa = l.IdEmpresa,
                    Endereco = l.Endereco,
                    Telefone = l.Telefone,
                    Status = l.Status,
                    ConfigDelivery = l.ConfigDelivery,
                    Empresa = l.Empresa
                })
                .FirstOrDefaultAsync(l => l.IdLoja == id);

            if (loja == null)
            {
                throw new KeyNotFoundException($"Loja com ID {id} não encontrada");
            }

            return new ResponseModel<Loja>(loja, "Loja obtida com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<Loja>>> ObterPorEmpresaIdAsync(int idEmpresa)
        {
            var lojas = await _context.Lojas
                .Include(l => l.Empresa)
                .Where(l => l.IdEmpresa == idEmpresa)
                .Select(l => new Loja
                {
                    IdLoja = l.IdLoja,
                    IdEmpresa = l.IdEmpresa,
                    Endereco = l.Endereco,
                    Telefone = l.Telefone,
                    Status = l.Status,
                    ConfigDelivery = l.ConfigDelivery,
                    Empresa = l.Empresa
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Loja>>(lojas, "Lojas obtidas com sucesso");
        }

        public async Task<ResponseModel<Loja>> CriarAsync(LojaCreateDto lojaDto)
        {
            if (lojaDto == null)
            {
                throw new ArgumentNullException(nameof(lojaDto), "Dados da loja não podem ser nulos");
            }

            // Verificar se a empresa existe e carregar dados
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.IdEmpresa == lojaDto.IdEmpresa);
            
            if (empresa == null)
            {
                throw new KeyNotFoundException($"Empresa com ID {lojaDto.IdEmpresa} não encontrada");
            }

            // Criar objeto Loja a partir do DTO
            var loja = new Loja
            {
                IdEmpresa = lojaDto.IdEmpresa,
                Endereco = lojaDto.Endereco,
                Telefone = lojaDto.Telefone,
                Status = lojaDto.Status,
                ConfigDelivery = lojaDto.ConfigDelivery
            };

            // Obter coordenadas e informações do endereço (cidade/UF) automaticamente
            var enderecoCompleto = await _geocodificacaoService.ObterEnderecoCompletoAsync(lojaDto.Endereco);
            
            if (enderecoCompleto.Status && enderecoCompleto.Dados != null)
            {
                loja.Latitude = enderecoCompleto.Dados.Latitude;
                loja.Longitude = enderecoCompleto.Dados.Longitude;
            }

            _context.Lojas.Add(loja);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(loja)
                .Reference(l => l.Empresa)
                .LoadAsync();

            // Criar bairros automaticamente se cidade e UF foram obtidos
            if (enderecoCompleto.Status && 
                enderecoCompleto.Dados != null && 
                !string.IsNullOrEmpty(enderecoCompleto.Dados.Cidade) && 
                !string.IsNullOrEmpty(enderecoCompleto.Dados.Uf))
            {
                try
                {
                    // Obter taxa por KM da empresa (ou usar padrão 7,50)
                    var taxaPorKm = empresa.TaxaPorKm ?? 7.50m;

                    // Criar bairros automaticamente com a taxa da empresa
                    var resultadoBairros = await _bairroService.CriarBairrosAutomaticamenteAsync(
                        loja.IdLoja,
                        enderecoCompleto.Dados.Cidade,
                        enderecoCompleto.Dados.Uf,
                        latitudeLoja: loja.Latitude,
                        longitudeLoja: loja.Longitude,
                        taxaPorKm: taxaPorKm
                    );

                    // Log do resultado (opcional)
                    if (!resultadoBairros.Status)
                    {
                        // Não falha a criação da loja se não conseguir criar bairros
                        // Apenas registra o erro
                    }
                }
                catch (Exception)
                {
                    // Não falha a criação da loja se houver erro ao criar bairros
                    // Os bairros podem ser criados manualmente depois
                }
            }

            return new ResponseModel<Loja>(loja, "Loja criada com sucesso");
        }

        public async Task<ResponseModel<Loja>> AtualizarAsync(int id, Loja loja)
        {
            if (loja == null)
            {
                throw new ArgumentNullException(nameof(loja), "Loja não pode ser nula");
            }

            var lojaExistente = await _context.Lojas.FindAsync(id);

            if (lojaExistente == null)
            {
                throw new KeyNotFoundException($"Loja com ID {id} não encontrada");
            }

            // Verificar se a empresa foi alterada e se existe
            if (loja.IdEmpresa != lojaExistente.IdEmpresa)
            {
                var empresaExiste = await _context.Empresas
                    .AnyAsync(e => e.IdEmpresa == loja.IdEmpresa);
                if (!empresaExiste)
                {
                    throw new KeyNotFoundException($"Empresa com ID {loja.IdEmpresa} não encontrada");
                }
            }

            lojaExistente.IdEmpresa = loja.IdEmpresa;
            lojaExistente.Endereco = loja.Endereco;
            lojaExistente.Telefone = loja.Telefone;
            lojaExistente.Status = loja.Status;
            lojaExistente.ConfigDelivery = loja.ConfigDelivery;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(lojaExistente)
                .Reference(l => l.Empresa)
                .LoadAsync();

            return new ResponseModel<Loja>(lojaExistente, "Loja atualizada com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var loja = await _context.Lojas.FindAsync(id);

            if (loja == null)
            {
                throw new KeyNotFoundException($"Loja com ID {id} não encontrada");
            }

            _context.Lojas.Remove(loja);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Loja deletada com sucesso");
        }
    }
}

