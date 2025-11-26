using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Produtos
{
    public class ProdutoVariacaoService : IProdutoVariacaoService
    {
        private readonly AppDbContext _context;

        public ProdutoVariacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<ProdutoVariacao>>> ObterTodosAsync()
        {
            var produtoVariacoes = await _context.ProdutoVariacoes
                .Include(pv => pv.Produto)
                .Include(pv => pv.Variacao)
                .Select(pv => new ProdutoVariacao
                {
                    IdProduto = pv.IdProduto,
                    IdVariacao = pv.IdVariacao,
                    Produto = pv.Produto,
                    Variacao = pv.Variacao
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<ProdutoVariacao>>(produtoVariacoes, "Vínculos produto-variação obtidos com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<ProdutoVariacao>>> ObterPorProdutoIdAsync(long idProduto)
        {
            var produtoVariacoes = await _context.ProdutoVariacoes
                .Include(pv => pv.Produto)
                .Include(pv => pv.Variacao)
                .Where(pv => pv.IdProduto == idProduto)
                .Select(pv => new ProdutoVariacao
                {
                    IdProduto = pv.IdProduto,
                    IdVariacao = pv.IdVariacao,
                    Produto = pv.Produto,
                    Variacao = pv.Variacao
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<ProdutoVariacao>>(produtoVariacoes, "Vínculos produto-variação obtidos com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<ProdutoVariacao>>> ObterPorVariacaoIdAsync(int idVariacao)
        {
            var produtoVariacoes = await _context.ProdutoVariacoes
                .Include(pv => pv.Produto)
                .Include(pv => pv.Variacao)
                .Where(pv => pv.IdVariacao == idVariacao)
                .Select(pv => new ProdutoVariacao
                {
                    IdProduto = pv.IdProduto,
                    IdVariacao = pv.IdVariacao,
                    Produto = pv.Produto,
                    Variacao = pv.Variacao
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<ProdutoVariacao>>(produtoVariacoes, "Vínculos produto-variação obtidos com sucesso");
        }

        public async Task<ResponseModel<ProdutoVariacao>> CriarAsync(ProdutoVariacaoCreateDto produtoVariacaoDto)
        {
            if (produtoVariacaoDto == null)
            {
                throw new ArgumentNullException(nameof(produtoVariacaoDto), "Dados do vínculo produto-variação não podem ser nulos");
            }

            // Verificar se o produto existe
            var produtoExiste = await _context.Produtos
                .AnyAsync(p => p.IdProduto == produtoVariacaoDto.IdProduto);
            if (!produtoExiste)
            {
                throw new KeyNotFoundException($"Produto com ID {produtoVariacaoDto.IdProduto} não encontrado");
            }

            // Verificar se a variação existe
            var variacaoExiste = await _context.Variacoes
                .AnyAsync(v => v.IdVariacao == produtoVariacaoDto.IdVariacao);
            if (!variacaoExiste)
            {
                throw new KeyNotFoundException($"Variação com ID {produtoVariacaoDto.IdVariacao} não encontrada");
            }

            // Verificar se o vínculo já existe
            var vinculoExistente = await _context.ProdutoVariacoes
                .AnyAsync(pv => pv.IdProduto == produtoVariacaoDto.IdProduto && pv.IdVariacao == produtoVariacaoDto.IdVariacao);
            if (vinculoExistente)
            {
                throw new ArgumentException($"Já existe um vínculo entre o produto {produtoVariacaoDto.IdProduto} e a variação {produtoVariacaoDto.IdVariacao}");
            }

            var produtoVariacao = new ProdutoVariacao
            {
                IdProduto = produtoVariacaoDto.IdProduto,
                IdVariacao = produtoVariacaoDto.IdVariacao
            };

            _context.ProdutoVariacoes.Add(produtoVariacao);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(produtoVariacao)
                .Reference(pv => pv.Produto)
                .LoadAsync();
            await _context.Entry(produtoVariacao)
                .Reference(pv => pv.Variacao)
                .LoadAsync();

            return new ResponseModel<ProdutoVariacao>(produtoVariacao, "Vínculo produto-variação criado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(long idProduto, int idVariacao)
        {
            var produtoVariacao = await _context.ProdutoVariacoes
                .FirstOrDefaultAsync(pv => pv.IdProduto == idProduto && pv.IdVariacao == idVariacao);

            if (produtoVariacao == null)
            {
                throw new KeyNotFoundException($"Vínculo entre produto {idProduto} e variação {idVariacao} não encontrado");
            }

            _context.ProdutoVariacoes.Remove(produtoVariacao);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Vínculo produto-variação deletado com sucesso");
        }
    }
}

