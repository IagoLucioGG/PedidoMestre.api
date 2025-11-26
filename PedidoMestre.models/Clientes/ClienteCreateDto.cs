using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Clientes
{
    /// <summary>
    /// DTO simplificado para criação de Cliente
    /// </summary>
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Senha { get; set; }
    }
}

