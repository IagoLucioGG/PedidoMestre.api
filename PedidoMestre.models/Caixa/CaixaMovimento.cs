using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Models.Caixa
{
    public class CaixaMovimento
    {
        [Key]
        public long IdMovimento { get; set; }

        public int IdLoja { get; set; }

        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        [MaxLength(255)]
        public string? Descricao { get; set; }

        [Required]
        public DateTime CriadoEm { get; set; }

        // Relacionamento: Um movimento pertence a uma loja
        public Loja Loja { get; set; }

        // Relacionamento: Um movimento pertence a um usuário
        public Usuario Usuario { get; set; }
    }
}

