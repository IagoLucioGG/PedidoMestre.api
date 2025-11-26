using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Services.Interfaces
{
    public interface IVariacaoService
    {
        Task<ResponseModel<IEnumerable<Variacao>>> ObterTodosAsync();
        Task<ResponseModel<Variacao>> ObterPorIdAsync(int id);
        Task<ResponseModel<Variacao>> CriarAsync(VariacaoCreateDto variacaoDto);
        Task<ResponseModel<Variacao>> AtualizarAsync(int id, Variacao variacao);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

