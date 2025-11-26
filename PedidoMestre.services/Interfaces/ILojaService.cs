using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Services.Interfaces
{
    public interface ILojaService
    {
        Task<ResponseModel<IEnumerable<Loja>>> ObterTodosAsync();
        Task<ResponseModel<Loja>> ObterPorIdAsync(int id);
        Task<ResponseModel<IEnumerable<Loja>>> ObterPorEmpresaIdAsync(int idEmpresa);
        Task<ResponseModel<Loja>> CriarAsync(LojaCreateDto lojaDto);
        Task<ResponseModel<Loja>> AtualizarAsync(int id, Loja loja);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

