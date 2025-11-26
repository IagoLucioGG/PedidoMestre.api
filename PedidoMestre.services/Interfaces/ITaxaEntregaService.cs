using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Services.Interfaces
{
    public interface ITaxaEntregaService
    {
        Task<ResponseModel<CalculoTaxaEntregaResultado>> CalcularTaxaEntregaAsync(long idEndereco, int? idLojaPreferida = null);
        Task<ResponseModel<IEnumerable<LojaDisponivel>>> ObterLojasDisponiveisAsync(long idEndereco);
    }

    public class CalculoTaxaEntregaResultado
    {
        public int IdLoja { get; set; }
        public string NomeLoja { get; set; } = string.Empty;
        public decimal TaxaEntrega { get; set; }
        public decimal DistanciaKm { get; set; }
        public bool LojaMaisProxima { get; set; }
        public string? Observacao { get; set; }
    }

    public class LojaDisponivel
    {
        public int IdLoja { get; set; }
        public string NomeLoja { get; set; } = string.Empty;
        public decimal TaxaEntrega { get; set; }
        public decimal DistanciaKm { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

