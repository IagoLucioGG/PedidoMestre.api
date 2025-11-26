using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<ResponseModel<IEnumerable<Produto>>> ObterTodosAsync();
        Task<ResponseModel<Produto>> ObterPorIdAsync(long id);
        Task<ResponseModel<IEnumerable<Produto>>> ObterPorLojaIdAsync(int idLoja);
        Task<ResponseModel<IEnumerable<Produto>>> ObterPorCategoriaIdAsync(int idCategoria);
        Task<ResponseModel<Produto>> CriarAsync(ProdutoCreateDto produtoDto);
        Task<ResponseModel<Produto>> AtualizarAsync(long id, Produto produto);
        Task<ResponseModel<bool>> DeletarAsync(long id);
    }
}

