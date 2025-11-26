using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Models.Pedidos
{
    public class KdsPedidoItem
    {
        [Key]
        public long IdKdsItem { get; set; }

        public long IdKdsPedido { get; set; }

        public long IdProduto { get; set; }

        [MaxLength(255)]
        public string? NomeProduto { get; set; }

        [MaxLength(100)]
        public string? Variacao { get; set; }

        public int? Quantidade { get; set; }

        [MaxLength(255)]
        public string? Observacao { get; set; }

        [MaxLength(50)]
        public string? StatusItem { get; set; }

        // Relacionamento: Um item do KDS pertence a um KDS pedido
        public KdsPedido KdsPedido { get; set; }

        // Relacionamento: Um item do KDS tem um produto
        public Produto Produto { get; set; }
    }
}

