using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Usuarios
{
    public class Perfil
    {
        [Key]
        public int IdPerfil { get; set; }
        [Required]
        public string NmPerfil { get; set; }
        public string Descricao { get; set; }

        public UsuarioPerfil UsuarioPerfil { get; set; }
    }
}

