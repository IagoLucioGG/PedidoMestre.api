using PedidoMestre.Models.Common;
using PedidoMestre.Models.Clientes;

namespace PedidoMestre.Services.Interfaces
{
    public interface IClienteService
    {
        Task<ResponseModel<IEnumerable<Cliente>>> ObterTodosAsync();
        Task<ResponseModel<Cliente>> ObterPorIdAsync(long id);
        Task<ResponseModel<Cliente>> CriarAsync(ClienteCreateDto clienteDto);
        Task<ResponseModel<Cliente>> AtualizarAsync(long id, Cliente cliente);
        Task<ResponseModel<bool>> DeletarAsync(long id);
    }
}

