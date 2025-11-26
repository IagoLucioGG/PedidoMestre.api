using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Clientes;

namespace PedidoMestre.Models.Geral
{
    public class Bairro
    {
        [Key]
        public int IdBairro { get; set; }

        public int IdLoja { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxaEntrega { get; set; }

        // Coordenadas do centro do bairro (opcional, para cálculo de distância)
        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        // Relacionamento: Um bairro pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento: Um bairro pode ter muitos endereços
        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
    }
}

