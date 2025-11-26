using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidoMestre.Models.Empresas
{
    public class Empresa
    {
        [Key]
        public int IdEmpresa { get; set; }

        [Required]
        [MaxLength(255)]
        public string NomeFantasia { get; set; }

        [Required]
        [MaxLength(18)]
        public string Cnpj { get; set; }

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        // Taxa por KM para cálculo de entrega (opcional, padrão: 7,50)
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TaxaPorKm { get; set; }

        // Relacionamento: Uma empresa tem muitas lojas
        public ICollection<Loja> Lojas { get; set; } = new List<Loja>();
    }
}

