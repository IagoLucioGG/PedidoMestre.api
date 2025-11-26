using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Usuarios
{
    /// <summary>
    /// DTO simplificado para criação de vínculo Usuario-Perfil
    /// </summary>
    public class UsuarioPerfilCreateDto
    {
        [Required(ErrorMessage = "IdUsuario é obrigatório")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "IdPerfil é obrigatório")]
        public int IdPerfil { get; set; }
    }
}

