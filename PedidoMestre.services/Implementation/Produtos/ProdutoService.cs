using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Produtos
{
    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _context;

        public ProdutoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Produto>>> ObterTodosAsync()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Loja)
                .Include(p => p.ProdutoVariacoes)
                    .ThenInclude(pv => pv.Variacao)
                .Select(p => new Produto
                {
                    IdProduto = p.IdProduto,
                    IdCategoria = p.IdCategoria,
                    IdLoja = p.IdLoja,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    PrecoBase = p.PrecoBase,
                    Ativo = p.Ativo,
                    TempoPreparoMin = p.TempoPreparoMin,
                    Categoria = p.Categoria,
                    Loja = p.Loja,
                    ProdutoVariacoes = p.ProdutoVariacoes
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Produto>>(produtos, "Produtos obtidos com sucesso");
        }

        public async Task<ResponseModel<Produto>> ObterPorIdAsync(long id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Loja)
                .Include(p => p.ProdutoVariacoes)
                    .ThenInclude(pv => pv.Variacao)
                .Select(p => new Produto
                {
                    IdProduto = p.IdProduto,
                    IdCategoria = p.IdCategoria,
                    IdLoja = p.IdLoja,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    PrecoBase = p.PrecoBase,
                    Ativo = p.Ativo,
                    TempoPreparoMin = p.TempoPreparoMin,
                    Categoria = p.Categoria,
                    Loja = p.Loja,
                    ProdutoVariacoes = p.ProdutoVariacoes
                })
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
            }

            return new ResponseModel<Produto>(produto, "Produto obtido com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<Produto>>> ObterPorLojaIdAsync(int idLoja)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Loja)
                .Include(p => p.ProdutoVariacoes)
                    .ThenInclude(pv => pv.Variacao)
                .Where(p => p.IdLoja == idLoja)
                .Select(p => new Produto
                {
                    IdProduto = p.IdProduto,
                    IdCategoria = p.IdCategoria,
                    IdLoja = p.IdLoja,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    PrecoBase = p.PrecoBase,
                    Ativo = p.Ativo,
                    TempoPreparoMin = p.TempoPreparoMin,
                    Categoria = p.Categoria,
                    Loja = p.Loja,
                    ProdutoVariacoes = p.ProdutoVariacoes
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Produto>>(produtos, "Produtos obtidos com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<Produto>>> ObterPorCategoriaIdAsync(int idCategoria)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Loja)
                .Include(p => p.ProdutoVariacoes)
                    .ThenInclude(pv => pv.Variacao)
                .Where(p => p.IdCategoria == idCategoria)
                .Select(p => new Produto
                {
                    IdProduto = p.IdProduto,
                    IdCategoria = p.IdCategoria,
                    IdLoja = p.IdLoja,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    PrecoBase = p.PrecoBase,
                    Ativo = p.Ativo,
                    TempoPreparoMin = p.TempoPreparoMin,
                    Categoria = p.Categoria,
                    Loja = p.Loja,
                    ProdutoVariacoes = p.ProdutoVariacoes
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Produto>>(produtos, "Produtos obtidos com sucesso");
        }

        public async Task<ResponseModel<Produto>> CriarAsync(ProdutoCreateDto produtoDto)
        {
            if (produtoDto == null)
            {
                throw new ArgumentNullException(nameof(produtoDto), "Dados do produto não podem ser nulos");
            }

            // Verificar se a categoria existe
            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == produtoDto.IdCategoria);
            if (!categoriaExiste)
            {
                throw new KeyNotFoundException($"Categoria com ID {produtoDto.IdCategoria} não encontrada");
            }

            // Verificar se a loja existe
            var lojaExiste = await _context.Lojas
                .AnyAsync(l => l.IdLoja == produtoDto.IdLoja);
            if (!lojaExiste)
            {
                throw new KeyNotFoundException($"Loja com ID {produtoDto.IdLoja} não encontrada");
            }

            var produto = new Produto
            {
                IdCategoria = produtoDto.IdCategoria,
                IdLoja = produtoDto.IdLoja,
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                PrecoBase = produtoDto.PrecoBase,
                Ativo = produtoDto.Ativo,
                TempoPreparoMin = produtoDto.TempoPreparoMin
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(produto)
                .Reference(p => p.Categoria)
                .LoadAsync();
            await _context.Entry(produto)
                .Reference(p => p.Loja)
                .LoadAsync();
            await _context.Entry(produto)
                .Collection(p => p.ProdutoVariacoes)
                .LoadAsync();

            return new ResponseModel<Produto>(produto, "Produto criado com sucesso");
        }

        public async Task<ResponseModel<Produto>> AtualizarAsync(long id, Produto produto)
        {
            if (produto == null)
            {
                throw new ArgumentNullException(nameof(produto), "Produto não pode ser nulo");
            }

            var produtoExistente = await _context.Produtos.FindAsync(id);

            if (produtoExistente == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
            }

            // Verificar se a categoria foi alterada e se existe
            if (produto.IdCategoria != produtoExistente.IdCategoria)
            {
                var categoriaExiste = await _context.Categorias
                    .AnyAsync(c => c.IdCategoria == produto.IdCategoria);
                if (!categoriaExiste)
                {
                    throw new KeyNotFoundException($"Categoria com ID {produto.IdCategoria} não encontrada");
                }
            }

            // Verificar se a loja foi alterada e se existe
            if (produto.IdLoja != produtoExistente.IdLoja)
            {
                var lojaExiste = await _context.Lojas
                    .AnyAsync(l => l.IdLoja == produto.IdLoja);
                if (!lojaExiste)
                {
                    throw new KeyNotFoundException($"Loja com ID {produto.IdLoja} não encontrada");
                }
            }

            produtoExistente.IdCategoria = produto.IdCategoria;
            produtoExistente.IdLoja = produto.IdLoja;
            produtoExistente.Nome = produto.Nome;
            produtoExistente.Descricao = produto.Descricao;
            produtoExistente.PrecoBase = produto.PrecoBase;
            produtoExistente.Ativo = produto.Ativo;
            produtoExistente.TempoPreparoMin = produto.TempoPreparoMin;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(produtoExistente)
                .Reference(p => p.Categoria)
                .LoadAsync();
            await _context.Entry(produtoExistente)
                .Reference(p => p.Loja)
                .LoadAsync();
            await _context.Entry(produtoExistente)
                .Collection(p => p.ProdutoVariacoes)
                .LoadAsync();

            return new ResponseModel<Produto>(produtoExistente, "Produto atualizado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(long id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Produto deletado com sucesso");
        }
    }
}

