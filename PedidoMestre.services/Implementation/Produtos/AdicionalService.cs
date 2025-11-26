using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Produtos
{
    public class AdicionalService : IAdicionalService
    {
        private readonly AppDbContext _context;

        public AdicionalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Adicional>>> ObterTodosAsync()
        {
            var adicionais = await _context.Adicionais
                .Include(a => a.Loja)
                .Select(a => new Adicional
                {
                    IdAdicional = a.IdAdicional,
                    IdLoja = a.IdLoja,
                    Nome = a.Nome,
                    Preco = a.Preco,
                    Loja = a.Loja
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Adicional>>(adicionais, "Adicionais obtidos com sucesso");
        }

        public async Task<ResponseModel<Adicional>> ObterPorIdAsync(int id)
        {
            var adicional = await _context.Adicionais
                .Include(a => a.Loja)
                .Select(a => new Adicional
                {
                    IdAdicional = a.IdAdicional,
                    IdLoja = a.IdLoja,
                    Nome = a.Nome,
                    Preco = a.Preco,
                    Loja = a.Loja
                })
                .FirstOrDefaultAsync(a => a.IdAdicional == id);

            if (adicional == null)
            {
                throw new KeyNotFoundException($"Adicional com ID {id} não encontrado");
            }

            return new ResponseModel<Adicional>(adicional, "Adicional obtido com sucesso");
        }

        public async Task<ResponseModel<IEnumerable<Adicional>>> ObterPorLojaIdAsync(int idLoja)
        {
            var adicionais = await _context.Adicionais
                .Include(a => a.Loja)
                .Where(a => a.IdLoja == idLoja)
                .Select(a => new Adicional
                {
                    IdAdicional = a.IdAdicional,
                    IdLoja = a.IdLoja,
                    Nome = a.Nome,
                    Preco = a.Preco,
                    Loja = a.Loja
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Adicional>>(adicionais, "Adicionais obtidos com sucesso");
        }

        public async Task<ResponseModel<Adicional>> CriarAsync(AdicionalCreateDto adicionalDto)
        {
            if (adicionalDto == null)
            {
                throw new ArgumentNullException(nameof(adicionalDto), "Dados do adicional não podem ser nulos");
            }

            // Verificar se a loja existe
            var lojaExiste = await _context.Lojas
                .AnyAsync(l => l.IdLoja == adicionalDto.IdLoja);
            if (!lojaExiste)
            {
                throw new KeyNotFoundException($"Loja com ID {adicionalDto.IdLoja} não encontrada");
            }

            var adicional = new Adicional
            {
                IdLoja = adicionalDto.IdLoja,
                Nome = adicionalDto.Nome,
                Preco = adicionalDto.Preco
            };

            _context.Adicionais.Add(adicional);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(adicional)
                .Reference(a => a.Loja)
                .LoadAsync();

            return new ResponseModel<Adicional>(adicional, "Adicional criado com sucesso");
        }

        public async Task<ResponseModel<Adicional>> AtualizarAsync(int id, Adicional adicional)
        {
            if (adicional == null)
            {
                throw new ArgumentNullException(nameof(adicional), "Adicional não pode ser nulo");
            }

            var adicionalExistente = await _context.Adicionais.FindAsync(id);

            if (adicionalExistente == null)
            {
                throw new KeyNotFoundException($"Adicional com ID {id} não encontrado");
            }

            // Verificar se a loja foi alterada e se existe
            if (adicional.IdLoja != adicionalExistente.IdLoja)
            {
                var lojaExiste = await _context.Lojas
                    .AnyAsync(l => l.IdLoja == adicional.IdLoja);
                if (!lojaExiste)
                {
                    throw new KeyNotFoundException($"Loja com ID {adicional.IdLoja} não encontrada");
                }
            }

            adicionalExistente.IdLoja = adicional.IdLoja;
            adicionalExistente.Nome = adicional.Nome;
            adicionalExistente.Preco = adicional.Preco;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(adicionalExistente)
                .Reference(a => a.Loja)
                .LoadAsync();

            return new ResponseModel<Adicional>(adicionalExistente, "Adicional atualizado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var adicional = await _context.Adicionais.FindAsync(id);

            if (adicional == null)
            {
                throw new KeyNotFoundException($"Adicional com ID {id} não encontrado");
            }

            _context.Adicionais.Remove(adicional);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Adicional deletado com sucesso");
        }
    }
}

