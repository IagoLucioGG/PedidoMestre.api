using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Models.Pedidos
{
    public class KdsLog
    {
        [Key]
        public long IdLog { get; set; }

        public long IdPedido { get; set; }

        public long? IdItem { get; set; }

        [MaxLength(50)]
        public string? StatusAntigo { get; set; }

        [MaxLength(50)]
        public string? StatusNovo { get; set; }

        public int? IdUsuario { get; set; }

        [MaxLength(50)]
        public string? Origem { get; set; }

        [MaxLength(255)]
        public string? Observacao { get; set; }

        public DateTime? RegistradoEm { get; set; }

        // Relacionamento: Um log pertence a um pedido
        public Pedido Pedido { get; set; }

        // Relacionamento: Um log pode ter um usuário
        public Usuario? Usuario { get; set; }
    }
}

