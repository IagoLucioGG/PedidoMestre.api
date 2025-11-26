using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Models.Produtos
{
    public class Adicional
    {
        [Key]
        public int IdAdicional { get; set; }

        public int IdLoja { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        // Relacionamento: Um adicional pertence a uma loja
        public Loja Loja { get; set; }
    }
}

