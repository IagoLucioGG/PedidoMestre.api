using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Clientes;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Models.Pedidos
{
    public class Pedido
    {
        [Key]
        public long IdPedido { get; set; }

        public int IdLoja { get; set; }

        public long IdCliente { get; set; }

        public long IdEndereco { get; set; }

        public int? IdEntregador { get; set; }

        [Required]
        [MaxLength(50)]
        public string Origem { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string FormaPagamento { get; set; }

        [Required]
        [MaxLength(50)]
        public string PagamentoStatus { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxaEntrega { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        public DateTime CriadoEm { get; set; }

        [Required]
        public DateTime AtualizadoEm { get; set; }

        // Relacionamento: Um pedido pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento: Um pedido pertence a um cliente
        public Cliente Cliente { get; set; }

        // Relacionamento: Um pedido tem um endereço de entrega
        public Endereco Endereco { get; set; }

        // Relacionamento: Um pedido pode ter um entregador
        public Usuario? Entregador { get; set; }

        // Relacionamento: Um pedido tem muitos itens
        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
    }
}

