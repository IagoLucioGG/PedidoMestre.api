using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Empresas
{
    public class EmpresaService : IEmpresaService
    {
        private readonly AppDbContext _context;
        private readonly ICnpjService _cnpjService;

        public EmpresaService(AppDbContext context, ICnpjService cnpjService)
        {
            _context = context;
            _cnpjService = cnpjService;
        }

        public async Task<ResponseModel<IEnumerable<Empresa>>> ObterTodosAsync()
        {
            var empresas = await _context.Empresas
                .Include(e => e.Lojas)
                .Select(e => new Empresa
                {
                    IdEmpresa = e.IdEmpresa,
                    NomeFantasia = e.NomeFantasia,
                    Cnpj = e.Cnpj,
                    LogoUrl = e.LogoUrl,
                    TaxaPorKm = e.TaxaPorKm,
                    Lojas = e.Lojas
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Empresa>>(empresas, "Empresas obtidas com sucesso");
        }

        public async Task<ResponseModel<Empresa>> ObterPorIdAsync(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Lojas)
                .Select(e => new Empresa
                {
                    IdEmpresa = e.IdEmpresa,
                    NomeFantasia = e.NomeFantasia,
                    Cnpj = e.Cnpj,
                    LogoUrl = e.LogoUrl,
                    TaxaPorKm = e.TaxaPorKm,
                    Lojas = e.Lojas
                })
                .FirstOrDefaultAsync(e => e.IdEmpresa == id);

            if (empresa == null)
            {
                throw new KeyNotFoundException($"Empresa com ID {id} não encontrada");
            }

            return new ResponseModel<Empresa>(empresa, "Empresa obtida com sucesso");
        }

        public async Task<ResponseModel<Empresa>> CriarAsync(EmpresaCreateDto empresaDto)
        {
            if (empresaDto == null)
            {
                throw new ArgumentNullException(nameof(empresaDto), "Dados da empresa não podem ser nulos");
            }

            // Verificar se já existe empresa com o mesmo CNPJ
            var cnpjExistente = await _context.Empresas
                .AnyAsync(e => e.Cnpj == empresaDto.Cnpj);
            if (cnpjExistente)
            {
                throw new ArgumentException($"Já existe uma empresa cadastrada com o CNPJ {empresaDto.Cnpj}");
            }

            // Validar CNPJ na Receita Federal
            var validacaoCnpj = await _cnpjService.ValidarCnpjAsync(empresaDto.Cnpj);
            if (!validacaoCnpj.Status || validacaoCnpj.Dados == null)
            {
                throw new ArgumentException($"CNPJ inválido: {validacaoCnpj.Mensagem}");
            }

            if (!validacaoCnpj.Dados.Valido)
            {
                throw new ArgumentException($"CNPJ não está ativo na Receita Federal. Situação: {validacaoCnpj.Dados.Situacao}");
            }

            // Criar objeto Empresa a partir do DTO
            var empresa = new Empresa
            {
                NomeFantasia = string.IsNullOrEmpty(empresaDto.NomeFantasia) && !string.IsNullOrEmpty(validacaoCnpj.Dados.NomeFantasia)
                    ? validacaoCnpj.Dados.NomeFantasia
                    : empresaDto.NomeFantasia,
                Cnpj = empresaDto.Cnpj,
                LogoUrl = empresaDto.LogoUrl,
                TaxaPorKm = empresaDto.TaxaPorKm
            };

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(empresa)
                .Collection(e => e.Lojas)
                .LoadAsync();

            return new ResponseModel<Empresa>(empresa, "Empresa criada com sucesso");
        }

        public async Task<ResponseModel<Empresa>> AtualizarAsync(int id, Empresa empresa)
        {
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa), "Empresa não pode ser nula");
            }

            var empresaExistente = await _context.Empresas.FindAsync(id);

            if (empresaExistente == null)
            {
                throw new KeyNotFoundException($"Empresa com ID {id} não encontrada");
            }

            // Verificar se o CNPJ foi alterado e se já existe outro cadastro com esse CNPJ
            if (empresa.Cnpj != empresaExistente.Cnpj)
            {
                var cnpjExistente = await _context.Empresas
                    .AnyAsync(e => e.Cnpj == empresa.Cnpj && e.IdEmpresa != id);
                if (cnpjExistente)
                {
                    throw new ArgumentException($"Já existe outra empresa cadastrada com o CNPJ {empresa.Cnpj}");
                }

                // Validar novo CNPJ na Receita Federal
                var validacaoCnpj = await _cnpjService.ValidarCnpjAsync(empresa.Cnpj);
                if (!validacaoCnpj.Status || validacaoCnpj.Dados == null)
                {
                    throw new ArgumentException($"CNPJ inválido: {validacaoCnpj.Mensagem}");
                }

                if (!validacaoCnpj.Dados.Valido)
                {
                    throw new ArgumentException($"CNPJ não está ativo na Receita Federal. Situação: {validacaoCnpj.Dados.Situacao}");
                }
            }

            empresaExistente.NomeFantasia = empresa.NomeFantasia;
            empresaExistente.Cnpj = empresa.Cnpj;
            empresaExistente.LogoUrl = empresa.LogoUrl;
            empresaExistente.TaxaPorKm = empresa.TaxaPorKm;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(empresaExistente)
                .Collection(e => e.Lojas)
                .LoadAsync();

            return new ResponseModel<Empresa>(empresaExistente, "Empresa atualizada com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Lojas)
                .FirstOrDefaultAsync(e => e.IdEmpresa == id);

            if (empresa == null)
            {
                throw new KeyNotFoundException($"Empresa com ID {id} não encontrada");
            }

            // Verificar se a empresa possui lojas vinculadas
            if (empresa.Lojas.Any())
            {
                throw new InvalidOperationException($"Não é possível deletar a empresa pois ela possui {empresa.Lojas.Count} loja(s) vinculada(s)");
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Empresa deletada com sucesso");
        }
    }
}

