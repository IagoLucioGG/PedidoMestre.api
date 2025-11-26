using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<ResponseModel<IEnumerable<Usuario>>> ObterTodosAsync();
        Task<ResponseModel<Usuario>> ObterPorIdAsync(int id);
        Task<ResponseModel<Usuario>> CriarAsync(UsuarioCreateDto usuarioDto);
        Task<ResponseModel<Usuario>> AtualizarAsync(int id, Usuario usuario);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

