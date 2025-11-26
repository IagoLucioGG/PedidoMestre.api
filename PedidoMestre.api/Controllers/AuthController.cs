using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para autenticação e login
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("0. Autenticação")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza login do cliente e retorna token JWT
        /// </summary>
        /// <param name="loginRequest">Credenciais de login (Email/Telefone e Senha)</param>
        /// <returns>Token JWT e informações do cliente</returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/Auth/login-cliente
        ///     {
        ///         "emailOuTelefone": "cliente@email.com",
        ///         "senha": "senha123"
        ///     }
        /// 
        /// O cliente pode fazer login usando email ou telefone.
        /// </remarks>
        [HttpPost("login-cliente")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<LoginResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseModel<LoginResponse>>> LoginCliente([FromBody] LoginRequest loginRequest)
        {
            var resultado = await _authService.LoginClienteAsync(loginRequest);
            
            if (!resultado.Status)
            {
                return Unauthorized(resultado);
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Realiza login do usuário (gestor) e retorna token JWT
        /// </summary>
        /// <param name="loginRequest">Credenciais de login (Email e Senha)</param>
        /// <returns>Token JWT e informações do usuário</returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/Auth/login-usuario
        ///     {
        ///         "emailOuTelefone": "gestor@empresa.com",
        ///         "senha": "senha123"
        ///     }
        /// 
        /// O usuário deve fazer login usando email.
        /// </remarks>
        [HttpPost("login-usuario")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<LoginResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseModel<LoginResponse>>> LoginUsuario([FromBody] LoginRequest loginRequest)
        {
            var resultado = await _authService.LoginUsuarioAsync(loginRequest);
            
            if (!resultado.Status)
            {
                return Unauthorized(resultado);
            }

            return Ok(resultado);
        }
    }
}

