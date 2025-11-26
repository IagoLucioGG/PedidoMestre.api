using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Geral;

namespace PedidoMestre.Models.Clientes
{
    public class Endereco
    {
        [Key]
        public long IdEndereco { get; set; }

        public long IdCliente { get; set; }

        public int IdBairro { get; set; }

        [Required]
        [MaxLength(255)]
        public string Logradouro { get; set; }

        [Required]
        [MaxLength(20)]
        public string Numero { get; set; }

        [MaxLength(100)]
        public string? Complemento { get; set; }

        [MaxLength(10)]
        public string? Cep { get; set; }

        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; }

        [Required]
        public bool Principal { get; set; }

        // Coordenadas para cálculo de distância
        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        // Relacionamento: Um endereço pertence a um cliente
        public Cliente Cliente { get; set; }

        // Relacionamento: Um endereço pertence a um bairro
        public Bairro Bairro { get; set; }
    }
}

