using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Services.Interfaces
{
    public interface IUsuarioPerfilService
    {
        Task<ResponseModel<IEnumerable<UsuarioPerfil>>> ObterTodosAsync();
        Task<ResponseModel<UsuarioPerfil>> ObterPorIdAsync(int id);
        Task<ResponseModel<UsuarioPerfil>> ObterPorUsuarioIdAsync(int idUsuario);
        Task<ResponseModel<UsuarioPerfil>> CriarAsync(UsuarioPerfilCreateDto usuarioPerfilDto);
        Task<ResponseModel<UsuarioPerfil>> AtualizarAsync(int id, UsuarioPerfil usuarioPerfil);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

