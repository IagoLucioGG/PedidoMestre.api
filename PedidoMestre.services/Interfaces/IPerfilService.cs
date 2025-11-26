using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Services.Interfaces
{
    public interface IPerfilService
    {
        Task<ResponseModel<IEnumerable<Perfil>>> ObterTodosAsync();
        Task<ResponseModel<Perfil>> ObterPorIdAsync(int id);
        Task<ResponseModel<Perfil>> CriarAsync(PerfilCreateDto perfilDto);
        Task<ResponseModel<Perfil>> AtualizarAsync(int id, Perfil perfil);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

