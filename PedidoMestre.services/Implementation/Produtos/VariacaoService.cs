using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Produtos
{
    public class VariacaoService : IVariacaoService
    {
        private readonly AppDbContext _context;

        public VariacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Variacao>>> ObterTodosAsync()
        {
            var variacoes = await _context.Variacoes
                .Include(v => v.ProdutoVariacoes)
                    .ThenInclude(pv => pv.Produto)
                .Select(v => new Variacao
                {
                    IdVariacao = v.IdVariacao,
                    Nome = v.Nome,
                    PrecoExtra = v.PrecoExtra,
                    ProdutoVariacoes = v.ProdutoVariacoes
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Variacao>>(variacoes, "Variações obtidas com sucesso");
        }

        public async Task<ResponseModel<Variacao>> ObterPorIdAsync(int id)
        {
            var variacao = await _context.Variacoes
                .Include(v => v.ProdutoVariacoes)
                    .ThenInclude(pv => pv.Produto)
                .Select(v => new Variacao
                {
                    IdVariacao = v.IdVariacao,
                    Nome = v.Nome,
                    PrecoExtra = v.PrecoExtra,
                    ProdutoVariacoes = v.ProdutoVariacoes
                })
                .FirstOrDefaultAsync(v => v.IdVariacao == id);

            if (variacao == null)
            {
                throw new KeyNotFoundException($"Variação com ID {id} não encontrada");
            }

            return new ResponseModel<Variacao>(variacao, "Variação obtida com sucesso");
        }

        public async Task<ResponseModel<Variacao>> CriarAsync(VariacaoCreateDto variacaoDto)
        {
            if (variacaoDto == null)
            {
                throw new ArgumentNullException(nameof(variacaoDto), "Dados da variação não podem ser nulos");
            }

            var variacao = new Variacao
            {
                Nome = variacaoDto.Nome,
                PrecoExtra = variacaoDto.PrecoExtra
            };

            _context.Variacoes.Add(variacao);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(variacao)
                .Collection(v => v.ProdutoVariacoes)
                .LoadAsync();

            return new ResponseModel<Variacao>(variacao, "Variação criada com sucesso");
        }

        public async Task<ResponseModel<Variacao>> AtualizarAsync(int id, Variacao variacao)
        {
            if (variacao == null)
            {
                throw new ArgumentNullException(nameof(variacao), "Variação não pode ser nula");
            }

            var variacaoExistente = await _context.Variacoes.FindAsync(id);

            if (variacaoExistente == null)
            {
                throw new KeyNotFoundException($"Variação com ID {id} não encontrada");
            }

            variacaoExistente.Nome = variacao.Nome;
            variacaoExistente.PrecoExtra = variacao.PrecoExtra;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(variacaoExistente)
                .Collection(v => v.ProdutoVariacoes)
                .LoadAsync();

            return new ResponseModel<Variacao>(variacaoExistente, "Variação atualizada com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var variacao = await _context.Variacoes
                .Include(v => v.ProdutoVariacoes)
                .FirstOrDefaultAsync(v => v.IdVariacao == id);

            if (variacao == null)
            {
                throw new KeyNotFoundException($"Variação com ID {id} não encontrada");
            }

            // Verificar se a variação está vinculada a produtos
            if (variacao.ProdutoVariacoes.Any())
            {
                throw new InvalidOperationException($"Não é possível deletar a variação pois ela está vinculada a {variacao.ProdutoVariacoes.Count} produto(s)");
            }

            _context.Variacoes.Remove(variacao);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Variação deletada com sucesso");
        }
    }
}

