using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Usuarios
{
    /// <summary>
    /// DTO simplificado para criação de Usuário
    /// </summary>
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "IdLoja é obrigatório")]
        public int IdLoja { get; set; }

        [Required(ErrorMessage = "Nome do usuário é obrigatório")]
        public string NmUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;

        public bool Status { get; set; } = true;
    }
}

