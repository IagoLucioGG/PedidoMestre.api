using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Usuario>>> ObterTodosAsync()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioPerfil)
                    .ThenInclude(up => up!.Perfil)
                .Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    NmUsuario = u.NmUsuario,
                    Email = u.Email,
                    Senha = u.Senha,
                    Status = u.Status,
                    IdLoja = u.IdLoja,
                    DataCriacao = u.DataCriacao,
                    UsuarioPerfil = u.UsuarioPerfil
                })
                .ToListAsync();
            
            return new ResponseModel<IEnumerable<Usuario>>(usuarios, "Usuários obtidos com sucesso");
        }

        public async Task<ResponseModel<Usuario>> ObterPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioPerfil)
                    .ThenInclude(up => up!.Perfil)
                .Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    NmUsuario = u.NmUsuario,
                    Email = u.Email,
                    Senha = u.Senha,
                    Status = u.Status,
                    IdLoja = u.IdLoja,
                    DataCriacao = u.DataCriacao,
                    UsuarioPerfil = u.UsuarioPerfil
                })
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            return new ResponseModel<Usuario>(usuario, "Usuário obtido com sucesso");
        }

        public async Task<ResponseModel<Usuario>> CriarAsync(UsuarioCreateDto usuarioDto)
        {
            if (usuarioDto == null)
            {
                throw new ArgumentNullException(nameof(usuarioDto), "Dados do usuário não podem ser nulos");
            }

            // Criar objeto Usuario a partir do DTO
            var usuario = new Usuario
            {
                IdLoja = usuarioDto.IdLoja,
                NmUsuario = usuarioDto.NmUsuario,
                Email = usuarioDto.Email,
                Senha = PedidoMestre.Services.Implementation.Clientes.AuthService.HashSenha(usuarioDto.Senha),
                Status = usuarioDto.Status,
                DataCriacao = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(usuario)
                .Reference(u => u.UsuarioPerfil)
                .LoadAsync();

            return new ResponseModel<Usuario>(usuario, "Usuário criado com sucesso");
        }

        public async Task<ResponseModel<Usuario>> AtualizarAsync(int id, Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "Usuário não pode ser nulo");
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            usuarioExistente.NmUsuario = usuario.NmUsuario;
            usuarioExistente.Email = usuario.Email;
            
            // Se a senha foi informada, fazer hash antes de atualizar
            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuarioExistente.Senha = PedidoMestre.Services.Implementation.Clientes.AuthService.HashSenha(usuario.Senha);
            }
            
            usuarioExistente.Status = usuario.Status;
            usuarioExistente.IdLoja = usuario.IdLoja;

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(usuarioExistente)
                .Reference(u => u.UsuarioPerfil)
                .LoadAsync();

            return new ResponseModel<Usuario>(usuarioExistente, "Usuário atualizado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Usuário deletado com sucesso");
        }
    }
}
