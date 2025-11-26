using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Clientes
{
    public class Cliente
    {
        [Key]
        public long IdCliente { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefone { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? SenhaHash { get; set; }

        // Relacionamento: Um cliente pode ter muitos endereços
        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
    }
}

