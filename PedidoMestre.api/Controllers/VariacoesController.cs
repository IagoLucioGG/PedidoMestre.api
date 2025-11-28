using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Produtos;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Variações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("8. Variações")]
    public class VariacoesController : ControllerBase
    {
        private readonly IVariacaoService _variacaoService;

        public VariacoesController(IVariacaoService variacaoService)
        {
            _variacaoService = variacaoService;
        }

        /// <summary>
        /// Lista todas as variações cadastradas
        /// </summary>
        /// <returns>Lista de variações</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Variacao>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Variacao>>>> ObterTodos()
        {
            var resultado = await _variacaoService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém uma variação pelo ID
        /// </summary>
        /// <param name="id">ID da variação</param>
        /// <returns>Dados da variação</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Variacao>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Variacao>>> ObterPorId(int id)
        {
            var resultado = await _variacaoService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria uma nova variação
        /// </summary>
        /// <param name="variacao">Dados da variação</param>
        /// <returns>Variação criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Variacao>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Variacao>>> Criar([FromBody] VariacaoCreateDto variacaoDto)
        {
            var resultado = await _variacaoService.CriarAsync(variacaoDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdVariacao }, resultado);
        }

        /// <summary>
        /// Atualiza uma variação existente
        /// </summary>
        /// <param name="id">ID da variação</param>
        /// <param name="variacao">Dados atualizados da variação</param>
        /// <returns>Variação atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Variacao>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Variacao>>> Atualizar(int id, [FromBody] Variacao variacao)
        {
            var resultado = await _variacaoService.AtualizarAsync(id, variacao);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta uma variação
        /// </summary>
        /// <param name="id">ID da variação</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _variacaoService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

