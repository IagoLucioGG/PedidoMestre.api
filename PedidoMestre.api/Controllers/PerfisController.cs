using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Usuarios;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Perfis
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("3. Usuários")]
    public class PerfisController : ControllerBase
    {
        private readonly IPerfilService _perfilService;

        public PerfisController(IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        /// <summary>
        /// Lista todos os perfis cadastrados
        /// </summary>
        /// <returns>Lista de perfis</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Perfil>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Perfil>>>> ObterTodos()
        {
            var resultado = await _perfilService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um perfil pelo ID
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <returns>Dados do perfil</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Perfil>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Perfil>>> ObterPorId(int id)
        {
            var resultado = await _perfilService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo perfil
        /// </summary>
        /// <param name="perfil">Dados do perfil</param>
        /// <returns>Perfil criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Perfil>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Perfil>>> Criar([FromBody] PerfilCreateDto perfilDto)
        {
            var resultado = await _perfilService.CriarAsync(perfilDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdPerfil }, resultado);
        }

        /// <summary>
        /// Atualiza um perfil existente
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <param name="perfil">Dados atualizados do perfil</param>
        /// <returns>Perfil atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Perfil>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Perfil>>> Atualizar(int id, [FromBody] Perfil perfil)
        {
            var resultado = await _perfilService.AtualizarAsync(id, perfil);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um perfil
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _perfilService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

