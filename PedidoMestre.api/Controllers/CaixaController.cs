using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Caixa;
using PedidoMestre.Models.Caixa;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Caixa
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("14. Caixa")]
    public class CaixaController : ControllerBase
    {
        private readonly ICaixaService _caixaService;

        public CaixaController(ICaixaService caixaService)
        {
            _caixaService = caixaService;
        }

        /// <summary>
        /// Lista os movimentos de caixa de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <returns>Lista de movimentos</returns>
        [HttpGet("movimentos/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<CaixaMovimentoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<CaixaMovimentoResponseDto>>>> ObterMovimentos(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _caixaService.ObterMovimentosAsync(idLoja, dataInicio, dataFim);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um movimento específico
        /// </summary>
        /// <param name="id">ID do movimento</param>
        /// <returns>Dados do movimento</returns>
        [HttpGet("movimento/{id}")]
        [ProducesResponseType(typeof(ResponseModel<CaixaMovimentoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<CaixaMovimentoResponseDto>>> ObterMovimento(long id)
        {
            var resultado = await _caixaService.ObterMovimentoAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo movimento de caixa
        /// </summary>
        /// <param name="movimentoDto">Dados do movimento</param>
        /// <returns>Movimento criado</returns>
        [HttpPost("movimento")]
        [ProducesResponseType(typeof(ResponseModel<CaixaMovimento>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<CaixaMovimento>>> CriarMovimento([FromBody] CaixaMovimentoCreateDto movimentoDto)
        {
            var resultado = await _caixaService.CriarMovimentoAsync(movimentoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return CreatedAtAction(nameof(ObterMovimento), new { id = resultado.Dados?.IdMovimento }, resultado);
        }

        /// <summary>
        /// Lista os fechamentos de caixa de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <returns>Lista de fechamentos</returns>
        [HttpGet("fechamentos/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<CaixaFechamentoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<CaixaFechamentoResponseDto>>>> ObterFechamentos(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _caixaService.ObterFechamentosAsync(idLoja, dataInicio, dataFim);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um fechamento específico
        /// </summary>
        /// <param name="id">ID do fechamento</param>
        /// <returns>Dados do fechamento</returns>
        [HttpGet("fechamento/{id}")]
        [ProducesResponseType(typeof(ResponseModel<CaixaFechamentoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<CaixaFechamentoResponseDto>>> ObterFechamento(int id)
        {
            var resultado = await _caixaService.ObterFechamentoAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo fechamento de caixa
        /// </summary>
        /// <param name="fechamentoDto">Dados do fechamento</param>
        /// <returns>Fechamento criado</returns>
        [HttpPost("fechamento")]
        [ProducesResponseType(typeof(ResponseModel<CaixaFechamento>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<CaixaFechamento>>> CriarFechamento([FromBody] CaixaFechamentoCreateDto fechamentoDto)
        {
            var resultado = await _caixaService.CriarFechamentoAsync(fechamentoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return CreatedAtAction(nameof(ObterFechamento), new { id = resultado.Dados?.IdFechamento }, resultado);
        }

        /// <summary>
        /// Obtém um resumo do caixa de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="data">Data para o resumo (opcional, padrão: hoje)</param>
        /// <returns>Resumo do caixa</returns>
        [HttpGet("resumo/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<CaixaResumoDto>), 200)]
        public async Task<ActionResult<ResponseModel<CaixaResumoDto>>> ObterResumo(int idLoja, [FromQuery] DateTime? data = null)
        {
            var resultado = await _caixaService.ObterResumoAsync(idLoja, data);
            return Ok(resultado);
        }
    }
}

