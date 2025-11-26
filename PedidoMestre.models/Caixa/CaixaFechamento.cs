using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Models.Caixa
{
    public class CaixaFechamento
    {
        [Key]
        public int IdFechamento { get; set; }

        public int IdLoja { get; set; }

        public int IdUsuario { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SaldoInicial { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SaldoFinal { get; set; }

        [Required]
        public DateTime Inicio { get; set; }

        [Required]
        public DateTime Fim { get; set; }

        // Relacionamento: Um fechamento pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento: Um fechamento pertence a um usuário
        public Usuario Usuario { get; set; }
    }
}

