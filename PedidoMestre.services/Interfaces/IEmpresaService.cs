using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Services.Interfaces
{
    public interface IEmpresaService
    {
        Task<ResponseModel<IEnumerable<Empresa>>> ObterTodosAsync();
        Task<ResponseModel<Empresa>> ObterPorIdAsync(int id);
        Task<ResponseModel<Empresa>> CriarAsync(EmpresaCreateDto empresaDto);
        Task<ResponseModel<Empresa>> AtualizarAsync(int id, Empresa empresa);
        Task<ResponseModel<bool>> DeletarAsync(int id);
    }
}

