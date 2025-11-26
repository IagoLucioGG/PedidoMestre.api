using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Pedidos;

namespace PedidoMestre.Models.Avaliacoes
{
    public class Avaliacao
    {
        [Key]
        public long IdAvaliacao { get; set; }

        public long IdPedido { get; set; }

        [Required]
        [Range(1, 5)]
        public int NotaPedido { get; set; }

        [Required]
        [Range(1, 5)]
        public int NotaPlataforma { get; set; }

        [MaxLength(500)]
        public string? Comentario { get; set; }

        [Required]
        public DateTime CriadoEm { get; set; }

        // Relacionamento: Uma avaliação pertence a um pedido
        public Pedido Pedido { get; set; }
    }
}

