using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Vínculos entre Usuários e Perfis
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("3. Usuários")]
    public class UsuarioPerfisController : ControllerBase
    {
        private readonly IUsuarioPerfilService _usuarioPerfilService;

        public UsuarioPerfisController(IUsuarioPerfilService usuarioPerfilService)
        {
            _usuarioPerfilService = usuarioPerfilService;
        }

        /// <summary>
        /// Lista todos os vínculos entre usuários e perfis
        /// </summary>
        /// <returns>Lista de vínculos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<UsuarioPerfil>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<UsuarioPerfil>>>> ObterTodos()
        {
            var resultado = await _usuarioPerfilService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um vínculo pelo ID
        /// </summary>
        /// <param name="id">ID do vínculo</param>
        /// <returns>Dados do vínculo</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<UsuarioPerfil>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<UsuarioPerfil>>> ObterPorId(int id)
        {
            var resultado = await _usuarioPerfilService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém o perfil de um usuário específico
        /// </summary>
        /// <param name="idUsuario">ID do usuário</param>
        /// <returns>Vínculo do usuário com seu perfil</returns>
        [HttpGet("usuario/{idUsuario}")]
        [ProducesResponseType(typeof(ResponseModel<UsuarioPerfil>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<UsuarioPerfil>>> ObterPorUsuario(int idUsuario)
        {
            var resultado = await _usuarioPerfilService.ObterPorUsuarioIdAsync(idUsuario);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo vínculo entre usuário e perfil
        /// </summary>
        /// <param name="usuarioPerfil">Dados do vínculo</param>
        /// <returns>Vínculo criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<UsuarioPerfil>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<UsuarioPerfil>>> Criar([FromBody] UsuarioPerfilCreateDto usuarioPerfilDto)
        {
            var resultado = await _usuarioPerfilService.CriarAsync(usuarioPerfilDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdUsuarioPerfil }, resultado);
        }

        /// <summary>
        /// Atualiza um vínculo existente
        /// </summary>
        /// <param name="id">ID do vínculo</param>
        /// <param name="usuarioPerfil">Dados atualizados do vínculo</param>
        /// <returns>Vínculo atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<UsuarioPerfil>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<UsuarioPerfil>>> Atualizar(int id, [FromBody] UsuarioPerfil usuarioPerfil)
        {
            var resultado = await _usuarioPerfilService.AtualizarAsync(id, usuarioPerfil);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um vínculo entre usuário e perfil
        /// </summary>
        /// <param name="id">ID do vínculo</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _usuarioPerfilService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

