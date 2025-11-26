using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Services.Interfaces
{
    public interface IProdutoVariacaoService
    {
        Task<ResponseModel<IEnumerable<ProdutoVariacao>>> ObterTodosAsync();
        Task<ResponseModel<IEnumerable<ProdutoVariacao>>> ObterPorProdutoIdAsync(long idProduto);
        Task<ResponseModel<IEnumerable<ProdutoVariacao>>> ObterPorVariacaoIdAsync(int idVariacao);
        Task<ResponseModel<ProdutoVariacao>> CriarAsync(ProdutoVariacaoCreateDto produtoVariacaoDto);
        Task<ResponseModel<bool>> DeletarAsync(long idProduto, int idVariacao);
    }
}

