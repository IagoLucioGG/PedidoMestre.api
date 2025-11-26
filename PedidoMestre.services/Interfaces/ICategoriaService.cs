using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<ResponseModel<IEnumerable<Categoria>>> ObterTodosAsync();
        Task<ResponseModel<Categoria>> ObterPorIdAsync(int id);
        Task<ResponseModel<IEnumerable<Categoria>>> ObterPorLojaIdAsync(int idLoja);
        Task<ResponseModel<Categoria>> CriarAsync(CategoriaCreateDto categoriaDto);
        Task<ResponseModel<Categoria>> AtualizarAsync(int id, Categoria categoria);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

