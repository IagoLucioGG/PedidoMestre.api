using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Produtos
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Categoria>>> ObterTodosAsync()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Loja)
                .Include(c => c.Produtos)
                .Select(c => new Categoria
                {
                    IdCategoria = c.IdCategoria,
                    IdLoja = c.IdLoja,
                    Nome = c.Nome,
                    Ordem = c.Ordem,
                    Loja = c.Loja,
                    Produtos = c.Produtos
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Categoria>>(categorias, "Categorias obtidas com sucesso");
        }

        public async Task<ResponseModel<Categoria>> ObterPorIdAsync(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Loja)
                .Include(c => c.Produtos)
                .Select(c => new Categoria
                {
                    IdCategoria = c.IdCategoria,
                    IdLoja = c.IdLoja,
                    Nome = c.Nome,
                    Ordem = c.Ordem,
                    Loja = c.Loja,
                    Produtos = c.Produtos
                })
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
            {
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada");
            }

            return new ResponseModel<Categoria>(categoria, "Categoria obtida com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<Categoria>>> ObterPorLojaIdAsync(int idLoja)
        {
            var categorias = await _context.Categorias
                .Include(c => c.Loja)
                .Include(c => c.Produtos)
                .Where(c => c.IdLoja == idLoja)
                .OrderBy(c => c.Ordem)
                .Select(c => new Categoria
                {
                    IdCategoria = c.IdCategoria,
                    IdLoja = c.IdLoja,
                    Nome = c.Nome,
                    Ordem = c.Ordem,
                    Loja = c.Loja,
                    Produtos = c.Produtos
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Categoria>>(categorias, "Categorias obtidas com sucesso");
        }

        public async Task<ResponseModel<Categoria>> CriarAsync(CategoriaCreateDto categoriaDto)
        {
            if (categoriaDto == null)
            {
                throw new ArgumentNullException(nameof(categoriaDto), "Dados da categoria não podem ser nulos");
            }

            // Verificar se a loja existe
            var lojaExiste = await _context.Lojas
                .AnyAsync(l => l.IdLoja == categoriaDto.IdLoja);
            if (!lojaExiste)
            {
                throw new KeyNotFoundException($"Loja com ID {categoriaDto.IdLoja} não encontrada");
            }

            var categoria = new Categoria
            {
                IdLoja = categoriaDto.IdLoja,
                Nome = categoriaDto.Nome,
                Ordem = categoriaDto.Ordem
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(categoria)
                .Reference(c => c.Loja)
                .LoadAsync();
            await _context.Entry(categoria)
                .Collection(c => c.Produtos)
                .LoadAsync();

            return new ResponseModel<Categoria>(categoria, "Categoria criada com sucesso");
        }

        public async Task<ResponseModel<Categoria>> AtualizarAsync(int id, Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentNullException(nameof(categoria), "Categoria não pode ser nula");
            }

            var categoriaExistente = await _context.Categorias.FindAsync(id);

            if (categoriaExistente == null)
            {
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada");
            }

            // Verificar se a loja foi alterada e se existe
            if (categoria.IdLoja != categoriaExistente.IdLoja)
            {
                var lojaExiste = await _context.Lojas
                    .AnyAsync(l => l.IdLoja == categoria.IdLoja);
                if (!lojaExiste)
                {
                    throw new KeyNotFoundException($"Loja com ID {categoria.IdLoja} não encontrada");
                }
            }

            categoriaExistente.IdLoja = categoria.IdLoja;
            categoriaExistente.Nome = categoria.Nome;
            categoriaExistente.Ordem = categoria.Ordem;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(categoriaExistente)
                .Reference(c => c.Loja)
                .LoadAsync();
            await _context.Entry(categoriaExistente)
                .Collection(c => c.Produtos)
                .LoadAsync();

            return new ResponseModel<Categoria>(categoriaExistente, "Categoria atualizada com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
            {
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada");
            }

            // Verificar se a categoria possui produtos vinculados
            if (categoria.Produtos.Any())
            {
                throw new InvalidOperationException($"Não é possível deletar a categoria pois ela possui {categoria.Produtos.Count} produto(s) vinculado(s)");
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Categoria deletada com sucesso");
        }
    }
}

