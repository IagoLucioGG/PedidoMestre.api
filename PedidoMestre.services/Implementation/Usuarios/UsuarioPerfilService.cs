using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Usuarios
{
    public class UsuarioPerfilService : IUsuarioPerfilService
    {
        private readonly AppDbContext _context;

        public UsuarioPerfilService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<UsuarioPerfil>>> ObterTodosAsync()
        {
            var usuariosPerfis = await _context.UsuariosPerfis
                .Include(up => up.Usuario)
                .Include(up => up.Perfil)
                .Select(up => new UsuarioPerfil
                {
                    IdUsuarioPerfil = up.IdUsuarioPerfil,
                    IdUsuario = up.IdUsuario,
                    IdPerfil = up.IdPerfil,
                    Usuario = up.Usuario,
                    Perfil = up.Perfil
                })
                .ToListAsync();
            
            return new ResponseModel<IEnumerable<UsuarioPerfil>>(usuariosPerfis, "Vínculos de usuário-perfil obtidos com sucesso");
        }

        public async Task<ResponseModel<UsuarioPerfil>> ObterPorIdAsync(int id)
        {
            var usuarioPerfil = await _context.UsuariosPerfis
                .Include(up => up.Usuario)
                .Include(up => up.Perfil)
                .Select(up => new UsuarioPerfil
                {
                    IdUsuarioPerfil = up.IdUsuarioPerfil,
                    IdUsuario = up.IdUsuario,
                    IdPerfil = up.IdPerfil,
                    Usuario = up.Usuario,
                    Perfil = up.Perfil
                })
                .FirstOrDefaultAsync(up => up.IdUsuarioPerfil == id);

            if (usuarioPerfil == null)
            {
                throw new KeyNotFoundException($"Vínculo de usuário-perfil com ID {id} não encontrado");
            }

            return new ResponseModel<UsuarioPerfil>(usuarioPerfil, "Vínculo de usuário-perfil obtido com sucesso");
        }

        public async Task<ResponseModel<UsuarioPerfil>> ObterPorUsuarioIdAsync(int idUsuario)
        {
            var usuarioPerfil = await _context.UsuariosPerfis
                .Include(up => up.Usuario)
                .Include(up => up.Perfil)
                .Select(up => new UsuarioPerfil
                {
                    IdUsuarioPerfil = up.IdUsuarioPerfil,
                    IdUsuario = up.IdUsuario,
                    IdPerfil = up.IdPerfil,
                    Usuario = up.Usuario,
                    Perfil = up.Perfil
                })
                .FirstOrDefaultAsync(up => up.IdUsuario == idUsuario);

            if (usuarioPerfil == null)
            {
                throw new KeyNotFoundException($"Vínculo de usuário-perfil para o usuário com ID {idUsuario} não encontrado");
            }

            return new ResponseModel<UsuarioPerfil>(usuarioPerfil, "Vínculo de usuário-perfil obtido com sucesso");
        }

        public async Task<ResponseModel<UsuarioPerfil>> CriarAsync(UsuarioPerfilCreateDto usuarioPerfilDto)
        {
            if (usuarioPerfilDto == null)
            {
                throw new ArgumentNullException(nameof(usuarioPerfilDto), "Dados do vínculo de usuário-perfil não podem ser nulos");
            }

            // Verificar se o usuário existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == usuarioPerfilDto.IdUsuario);
            if (!usuarioExiste)
            {
                throw new KeyNotFoundException($"Usuário com ID {usuarioPerfilDto.IdUsuario} não encontrado");
            }

            // Verificar se o perfil existe
            var perfilExiste = await _context.Perfis.AnyAsync(p => p.IdPerfil == usuarioPerfilDto.IdPerfil);
            if (!perfilExiste)
            {
                throw new KeyNotFoundException($"Perfil com ID {usuarioPerfilDto.IdPerfil} não encontrado");
            }

            // Verificar se já existe vínculo para este usuário
            var vinculoExistente = await _context.UsuariosPerfis
                .AnyAsync(up => up.IdUsuario == usuarioPerfilDto.IdUsuario);
            if (vinculoExistente)
            {
                throw new ArgumentException($"Já existe um vínculo de perfil para o usuário com ID {usuarioPerfilDto.IdUsuario}");
            }

            var usuarioPerfil = new UsuarioPerfil
            {
                IdUsuario = usuarioPerfilDto.IdUsuario,
                IdPerfil = usuarioPerfilDto.IdPerfil
            };

            _context.UsuariosPerfis.Add(usuarioPerfil);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos
            await _context.Entry(usuarioPerfil)
                .Reference(up => up.Usuario)
                .LoadAsync();
            await _context.Entry(usuarioPerfil)
                .Reference(up => up.Perfil)
                .LoadAsync();

            return new ResponseModel<UsuarioPerfil>(usuarioPerfil, "Vínculo de usuário-perfil criado com sucesso");
        }

        public async Task<ResponseModel<UsuarioPerfil>> AtualizarAsync(int id, UsuarioPerfil usuarioPerfil)
        {
            if (usuarioPerfil == null)
            {
                throw new ArgumentNullException(nameof(usuarioPerfil), "Vínculo de usuário-perfil não pode ser nulo");
            }

            var usuarioPerfilExistente = await _context.UsuariosPerfis.FindAsync(id);

            if (usuarioPerfilExistente == null)
            {
                throw new KeyNotFoundException($"Vínculo de usuário-perfil com ID {id} não encontrado");
            }

            // Verificar se o perfil existe (se foi alterado)
            if (usuarioPerfil.IdPerfil != usuarioPerfilExistente.IdPerfil)
            {
                var perfilExiste = await _context.Perfis.AnyAsync(p => p.IdPerfil == usuarioPerfil.IdPerfil);
                if (!perfilExiste)
                {
                    throw new KeyNotFoundException($"Perfil com ID {usuarioPerfil.IdPerfil} não encontrado");
                }
            }

            usuarioPerfilExistente.IdPerfil = usuarioPerfil.IdPerfil;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos
            await _context.Entry(usuarioPerfilExistente)
                .Reference(up => up.Usuario)
                .LoadAsync();
            await _context.Entry(usuarioPerfilExistente)
                .Reference(up => up.Perfil)
                .LoadAsync();

            return new ResponseModel<UsuarioPerfil>(usuarioPerfilExistente, "Vínculo de usuário-perfil atualizado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var usuarioPerfil = await _context.UsuariosPerfis.FindAsync(id);

            if (usuarioPerfil == null)
            {
                throw new KeyNotFoundException($"Vínculo de usuário-perfil com ID {id} não encontrado");
            }

            _context.UsuariosPerfis.Remove(usuarioPerfil);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Vínculo de usuário-perfil deletado com sucesso");
        }
    }
}

