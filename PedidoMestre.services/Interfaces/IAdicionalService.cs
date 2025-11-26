using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Services.Interfaces
{
    public interface IAdicionalService
    {
        Task<ResponseModel<IEnumerable<Adicional>>> ObterTodosAsync();
        Task<ResponseModel<Adicional>> ObterPorIdAsync(int id);
        Task<ResponseModel<IEnumerable<Adicional>>> ObterPorLojaIdAsync(int idLoja);
        Task<ResponseModel<Adicional>> CriarAsync(AdicionalCreateDto adicionalDto);
        Task<ResponseModel<Adicional>> AtualizarAsync(int id, Adicional adicional);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

