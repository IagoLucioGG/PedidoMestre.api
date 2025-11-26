using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PedidoMestre.Services.Implementation.Clientes
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ResponseModel<LoginResponse>> LoginClienteAsync(LoginRequest loginRequest)
        {
            // Buscar cliente por email ou telefone
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    (c.Email != null && c.Email == loginRequest.EmailOuTelefone) ||
                    c.Telefone == loginRequest.EmailOuTelefone);

            if (cliente == null)
            {
                return new ResponseModel<LoginResponse>("Email/Telefone ou senha inválidos");
            }

            // Verificar se o cliente tem senha cadastrada
            if (string.IsNullOrEmpty(cliente.SenhaHash))
            {
                return new ResponseModel<LoginResponse>("Cliente não possui senha cadastrada. Por favor, cadastre uma senha primeiro.");
            }

            // Verificar senha
            if (!VerificarSenha(loginRequest.Senha, cliente.SenhaHash))
            {
                return new ResponseModel<LoginResponse>("Email/Telefone ou senha inválidos");
            }

            // Gerar token JWT
            var token = GerarTokenJwtCliente(cliente.IdCliente, cliente.Email ?? cliente.Telefone);
            var expiraEm = DateTime.UtcNow.AddHours(24); // Token válido por 24 horas

            var loginResponse = new LoginResponse
            {
                Token = token,
                ExpiraEm = expiraEm,
                IdCliente = cliente.IdCliente,
                Nome = cliente.Nome,
                Email = cliente.Email,
                TipoUsuario = "Cliente"
            };

            return new ResponseModel<LoginResponse>(loginResponse, "Login realizado com sucesso");
        }

        public async Task<ResponseModel<LoginResponse>> LoginUsuarioAsync(LoginRequest loginRequest)
        {
            // Buscar usuário por email
            var usuario = await _context.Usuarios
                .Include(u => u.Loja)
                .FirstOrDefaultAsync(u => u.Email == loginRequest.EmailOuTelefone);

            if (usuario == null)
            {
                return new ResponseModel<LoginResponse>("Email ou senha inválidos");
            }

            // Verificar se o usuário está ativo
            if (!usuario.Status)
            {
                return new ResponseModel<LoginResponse>("Usuário inativo. Entre em contato com o administrador.");
            }

            // Verificar se o usuário tem senha cadastrada
            if (string.IsNullOrEmpty(usuario.Senha))
            {
                return new ResponseModel<LoginResponse>("Usuário não possui senha cadastrada. Por favor, cadastre uma senha primeiro.");
            }

            // Verificar senha
            if (!VerificarSenha(loginRequest.Senha, usuario.Senha))
            {
                return new ResponseModel<LoginResponse>("Email ou senha inválidos");
            }

            // Gerar token JWT
            var token = GerarTokenJwtUsuario(usuario.IdUsuario, usuario.Email, usuario.IdLoja);
            var expiraEm = DateTime.UtcNow.AddHours(24); // Token válido por 24 horas

            var loginResponse = new LoginResponse
            {
                Token = token,
                ExpiraEm = expiraEm,
                IdUsuario = usuario.IdUsuario,
                IdLoja = usuario.IdLoja,
                Nome = usuario.NmUsuario,
                Email = usuario.Email,
                TipoUsuario = "Usuario"
            };

            return new ResponseModel<LoginResponse>(loginResponse, "Login realizado com sucesso");
        }

        public string GerarTokenJwtCliente(long idCliente, string email)
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? "PedidoMestre_SecretKey_SuperSegura_2024_Minimo32Caracteres";
            var issuer = _configuration["Jwt:Issuer"] ?? "PedidoMestre";
            var audience = _configuration["Jwt:Audience"] ?? "PedidoMestreClients";
            var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, idCliente.ToString()),
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim("IdCliente", idCliente.ToString()),
                new Claim("TipoUsuario", "Cliente"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GerarTokenJwtUsuario(int idUsuario, string email, int idLoja)
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? "PedidoMestre_SecretKey_SuperSegura_2024_Minimo32Caracteres";
            var issuer = _configuration["Jwt:Issuer"] ?? "PedidoMestre";
            var audience = _configuration["Jwt:Audience"] ?? "PedidoMestreClients";
            var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim("IdUsuario", idUsuario.ToString()),
                new Claim("IdLoja", idLoja.ToString()),
                new Claim("TipoUsuario", "Usuario"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerificarSenha(string senha, string senhaHash)
        {
            // Implementação básica de verificação de senha
            // Assumindo que a senha foi hasheada com SHA256 (pode ser melhorado com BCrypt)
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                var hash = Convert.ToBase64String(hashedBytes);
                return hash == senhaHash;
            }
        }

        public static string HashSenha(string senha)
        {
            // Método auxiliar para hash de senha (usar no cadastro)
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

