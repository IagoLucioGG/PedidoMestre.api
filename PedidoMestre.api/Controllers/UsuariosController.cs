using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("3. Usuários")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Lista todos os usuários cadastrados
        /// </summary>
        /// <returns>Lista de usuários</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Usuario>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Usuario>>>> ObterTodos()
        {
            var resultado = await _usuarioService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Usuario>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Usuario>>> ObterPorId(int id)
        {
            var resultado = await _usuarioService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="usuarioDto">Dados básicos do usuário</param>
        /// <returns>Usuário criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Usuario>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Usuario>>> Criar([FromBody] UsuarioCreateDto usuarioDto)
        {
            var resultado = await _usuarioService.CriarAsync(usuarioDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdUsuario }, resultado);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="usuario">Dados atualizados do usuário</param>
        /// <returns>Usuário atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Usuario>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Usuario>>> Atualizar(int id, [FromBody] Usuario usuario)
        {
            var resultado = await _usuarioService.AtualizarAsync(id, usuario);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _usuarioService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

