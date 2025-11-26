using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Models.Geral{
    public class Taxas {
        [Key]
        public int IdTaxa { get; set; }
        public int IdBairro { get; set; }
        public decimal TaxaEntregaPadrão { get; set; }
        
        public Bairro Bairro { get; set; }
    }
}