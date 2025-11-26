using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Usuarios
{
    public class UsuarioPerfil
    {
        [Key]
        public int IdUsuarioPerfil { get; set; }

        public int IdUsuario { get; set; }

        public Usuario? Usuario { get; set; }
        public int IdPerfil { get; set; }
        public Perfil? Perfil { get; set; }
    }
}

