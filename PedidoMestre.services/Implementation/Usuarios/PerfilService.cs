using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Usuarios
{
    public class PerfilService : IPerfilService
    {
        private readonly AppDbContext _context;

        public PerfilService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Perfil>>> ObterTodosAsync()
        {
            var perfis = await _context.Perfis
                .Include(p => p.UsuarioPerfil)
                    .ThenInclude(up => up.Usuario)
                .Select(p => new Perfil
                {
                    IdPerfil = p.IdPerfil,
                    NmPerfil = p.NmPerfil,
                    Descricao = p.Descricao,
                    UsuarioPerfil = p.UsuarioPerfil
                })
                .ToListAsync();
            
            return new ResponseModel<IEnumerable<Perfil>>(perfis, "Perfis obtidos com sucesso");
        }

        public async Task<ResponseModel<Perfil>> ObterPorIdAsync(int id)
        {
            var perfil = await _context.Perfis
                .Include(p => p.UsuarioPerfil)
                    .ThenInclude(up => up.Usuario)
                .Select(p => new Perfil
                {
                    IdPerfil = p.IdPerfil,
                    NmPerfil = p.NmPerfil,
                    Descricao = p.Descricao,
                    UsuarioPerfil = p.UsuarioPerfil
                })
                .FirstOrDefaultAsync(p => p.IdPerfil == id);

            if (perfil == null)
            {
                throw new KeyNotFoundException($"Perfil com ID {id} não encontrado");
            }

            return new ResponseModel<Perfil>(perfil, "Perfil obtido com sucesso");
        }

        public async Task<ResponseModel<Perfil>> CriarAsync(PerfilCreateDto perfilDto)
        {
            if (perfilDto == null)
            {
                throw new ArgumentNullException(nameof(perfilDto), "Dados do perfil não podem ser nulos");
            }

            var perfil = new Perfil
            {
                NmPerfil = perfilDto.NmPerfil,
                Descricao = perfilDto.Descricao
            };

            _context.Perfis.Add(perfil);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(perfil)
                .Reference(p => p.UsuarioPerfil)
                .LoadAsync();

            return new ResponseModel<Perfil>(perfil, "Perfil criado com sucesso");
        }

        public async Task<ResponseModel<Perfil>> AtualizarAsync(int id, Perfil perfil)
        {
            if (perfil == null)
            {
                throw new ArgumentNullException(nameof(perfil), "Perfil não pode ser nulo");
            }

            var perfilExistente = await _context.Perfis.FindAsync(id);

            if (perfilExistente == null)
            {
                throw new KeyNotFoundException($"Perfil com ID {id} não encontrado");
            }

            perfilExistente.NmPerfil = perfil.NmPerfil;
            perfilExistente.Descricao = perfil.Descricao;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(perfilExistente)
                .Reference(p => p.UsuarioPerfil)
                .LoadAsync();

            return new ResponseModel<Perfil>(perfilExistente, "Perfil atualizado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var perfil = await _context.Perfis.FindAsync(id);

            if (perfil == null)
            {
                throw new KeyNotFoundException($"Perfil com ID {id} não encontrado");
            }

            _context.Perfis.Remove(perfil);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Perfil deletado com sucesso");
        }
    }
}

