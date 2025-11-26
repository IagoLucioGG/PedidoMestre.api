namespace PedidoMestre.Models.Common
{
    /// <summary>
    /// DTO para resposta de login (Cliente ou Usuário)
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        
        // Para Cliente
        public long? IdCliente { get; set; }
        
        // Para Usuário
        public int? IdUsuario { get; set; }
        public int? IdLoja { get; set; }
        
        // Campos comuns
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string TipoUsuario { get; set; } = string.Empty; // "Cliente" ou "Usuario"
    }
}

