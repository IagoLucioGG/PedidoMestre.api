using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Common
{
    /// <summary>
    /// DTO para requisição de login
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email ou Telefone é obrigatório")]
        public string EmailOuTelefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}

