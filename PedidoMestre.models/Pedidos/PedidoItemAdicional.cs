using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Produtos;

namespace PedidoMestre.Models.Pedidos
{
    public class PedidoItemAdicional
    {
        public long IdPedidoItem { get; set; }

        public int IdAdicional { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        // Relacionamento: PedidoItemAdicional pertence a um item do pedido
        public PedidoItem PedidoItem { get; set; }

        // Relacionamento: PedidoItemAdicional pertence a um adicional
        public Adicional Adicional { get; set; }
    }
}

