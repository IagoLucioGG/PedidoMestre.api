using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Models.Produtos
{
    public class Produto
    {
        [Key]
        public long IdProduto { get; set; }

        public int IdCategoria { get; set; }

        public int IdLoja { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoBase { get; set; }

        [Required]
        public bool Ativo { get; set; }

        public int? TempoPreparoMin { get; set; }

        // Relacionamento: Um produto pertence a uma categoria
        public Categoria Categoria { get; set; }

        // Relacionamento: Um produto pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento N:N: Um produto pode ter muitas variações
        public ICollection<ProdutoVariacao> ProdutoVariacoes { get; set; } = new List<ProdutoVariacao>();
    }
}

