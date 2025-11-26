using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Usuarios
{
    /// <summary>
    /// DTO simplificado para criação de Perfil
    /// </summary>
    public class PerfilCreateDto
    {
        [Required(ErrorMessage = "Nome do perfil é obrigatório")]
        public string NmPerfil { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Descricao { get; set; } = string.Empty;
    }
}

