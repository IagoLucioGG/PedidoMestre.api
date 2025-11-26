using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidoMestre.Models.Produtos
{
    public class Variacao
    {
        [Key]
        public int IdVariacao { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecoExtra { get; set; }

        // Relacionamento N:N: Uma variação pode estar em muitos produtos
        public ICollection<ProdutoVariacao> ProdutoVariacoes { get; set; } = new List<ProdutoVariacao>();
    }
}

