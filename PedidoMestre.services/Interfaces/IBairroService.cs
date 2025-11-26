using PedidoMestre.Models.Common;
using PedidoMestre.Models.Geral;

namespace PedidoMestre.Services.Interfaces
{
    public interface IBairroService
    {
        Task<ResponseModel<IEnumerable<Bairro>>> BuscarBairrosPorCidadeAsync(string cidade, string uf);
        Task<ResponseModel<IEnumerable<Bairro>>> CriarBairrosAutomaticamenteAsync(int idLoja, string cidade, string uf, decimal? latitudeLoja = null, decimal? longitudeLoja = null, decimal? taxaPorKm = null);
        Task<ResponseModel<Bairro>> AtualizarTaxaAsync(int idBairro, decimal novaTaxa);
    }
}

