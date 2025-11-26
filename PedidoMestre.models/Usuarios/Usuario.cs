using System.ComponentModel.DataAnnotations;
using PedidoMestre.Models.Empresas;

namespace PedidoMestre.Models.Usuarios
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdLoja { get; set; }
        public string NmUsuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Status { get; set; } = true;
        public DateTime DataCriacao { get; set; }

        // Relacionamento: Um usuário pertence a uma loja
        public Loja Loja { get; set; }

        public UsuarioPerfil UsuarioPerfil { get; set; }
    }
}

