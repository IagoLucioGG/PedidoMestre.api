using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Models.Pedidos
{
    public class PedidoItem
    {
        [Key]
        public long IdPedidoItem { get; set; }

        public long IdPedido { get; set; }

        public long IdProduto { get; set; }

        public int? IdVariacao { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorUnitario { get; set; }

        [MaxLength(255)]
        public string? Observacao { get; set; }

        // Relacionamento: Um item pertence a um pedido
        public Pedido Pedido { get; set; }

        // Relacionamento: Um item tem um produto
        public Produto Produto { get; set; }

        // Relacionamento: Um item pode ter uma variação
        public Variacao? Variacao { get; set; }

        // Relacionamento: Um item pode ter muitos adicionais
        public ICollection<PedidoItemAdicional> Adicionais { get; set; } = new List<PedidoItemAdicional>();
    }
}

