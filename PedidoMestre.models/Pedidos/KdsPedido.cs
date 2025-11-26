using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Models.Pedidos
{
    public class KdsPedido
    {
        [Key]
        public long IdKdsPedido { get; set; }

        public long IdPedido { get; set; }

        public int IdLoja { get; set; }

        [MaxLength(50)]
        public string? StatusCozinha { get; set; }

        public int? Prioridade { get; set; }

        public int? TempoEstimadoPreparo { get; set; }

        public int? TempoRealPreparo { get; set; }

        public DateTime? CriadoEm { get; set; }

        public DateTime? IniciadoEm { get; set; }

        public DateTime? FinalizadoEm { get; set; }

        // Relacionamento: Um KDS pedido pertence a um pedido
        public Pedido Pedido { get; set; }

        // Relacionamento: Um KDS pedido pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento: Um KDS pedido tem muitos itens
        public ICollection<KdsPedidoItem> Itens { get; set; } = new List<KdsPedidoItem>();
    }
}

