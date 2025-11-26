using PedidoMestre.Models.Common;

namespace PedidoMestre.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseModel<LoginResponse>> LoginClienteAsync(LoginRequest loginRequest);
        Task<ResponseModel<LoginResponse>> LoginUsuarioAsync(LoginRequest loginRequest);
        string GerarTokenJwtCliente(long idCliente, string email);
        string GerarTokenJwtUsuario(int idUsuario, string email, int idLoja);
    }
}

