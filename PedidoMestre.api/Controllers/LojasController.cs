using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Lojas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("2. Lojas")]
    public class LojasController : ControllerBase
    {
        private readonly ILojaService _lojaService;

        public LojasController(ILojaService lojaService)
        {
            _lojaService = lojaService;
        }

        /// <summary>
        /// Lista todas as lojas cadastradas
        /// </summary>
        /// <returns>Lista de lojas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Loja>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Loja>>>> ObterTodos()
        {
            var resultado = await _lojaService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém uma loja pelo ID
        /// </summary>
        /// <param name="id">ID da loja</param>
        /// <returns>Dados da loja</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Loja>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Loja>>> ObterPorId(int id)
        {
            var resultado = await _lojaService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todas as lojas de uma empresa
        /// </summary>
        /// <param name="idEmpresa">ID da empresa</param>
        /// <returns>Lista de lojas da empresa</returns>
        [HttpGet("empresa/{idEmpresa}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Loja>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Loja>>>> ObterPorEmpresa(int idEmpresa)
        {
            var resultado = await _lojaService.ObterPorEmpresaIdAsync(idEmpresa);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria uma nova loja. Os bairros serão criados automaticamente baseado no endereço informado.
        /// </summary>
        /// <param name="lojaDto">Dados básicos da loja</param>
        /// <returns>Loja criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Loja>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Loja>>> Criar([FromBody] LojaCreateDto lojaDto)
        {
            var resultado = await _lojaService.CriarAsync(lojaDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdLoja }, resultado);
        }

        /// <summary>
        /// Atualiza uma loja existente
        /// </summary>
        /// <param name="id">ID da loja</param>
        /// <param name="loja">Dados atualizados da loja</param>
        /// <returns>Loja atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Loja>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Loja>>> Atualizar(int id, [FromBody] Loja loja)
        {
            var resultado = await _lojaService.AtualizarAsync(id, loja);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta uma loja
        /// </summary>
        /// <param name="id">ID da loja</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _lojaService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

