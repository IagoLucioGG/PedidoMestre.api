using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Models.Produtos
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        public int IdLoja { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        public int Ordem { get; set; }

        // Relacionamento: Uma categoria pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento: Uma categoria tem muitos produtos
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}

