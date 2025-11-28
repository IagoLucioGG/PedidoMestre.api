using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Empresas;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Formas de Pagamento
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("3. Formas de Pagamento")]
    public class FormasPagamentoController : ControllerBase
    {
        private readonly IFormaPagamentoService _formaPagamentoService;

        public FormasPagamentoController(IFormaPagamentoService formaPagamentoService)
        {
            _formaPagamentoService = formaPagamentoService;
        }

        /// <summary>
        /// Lista todas as formas de pagamento de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de formas de pagamento</returns>
        [HttpGet("loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<FormaPagamentoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<FormaPagamentoResponseDto>>>> ObterTodos(int idLoja)
        {
            var resultado = await _formaPagamentoService.ObterTodosAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista formas de pagamento por tipo (Online ou Presencial)
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="tipo">Tipo: Online ou Presencial</param>
        /// <returns>Lista de formas de pagamento do tipo especificado</returns>
        [HttpGet("loja/{idLoja}/tipo/{tipo}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<FormaPagamentoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<FormaPagamentoResponseDto>>>> ObterPorTipo(int idLoja, string tipo)
        {
            var resultado = await _formaPagamentoService.ObterPorTipoAsync(idLoja, tipo);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém uma forma de pagamento pelo ID
        /// </summary>
        /// <param name="id">ID da forma de pagamento</param>
        /// <returns>Dados da forma de pagamento</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<FormaPagamentoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<FormaPagamentoResponseDto>>> ObterPorId(int id)
        {
            var resultado = await _formaPagamentoService.ObterPorIdAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria uma nova forma de pagamento
        /// </summary>
        /// <param name="formaPagamentoDto">Dados da forma de pagamento</param>
        /// <returns>Forma de pagamento criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<FormaPagamento>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<FormaPagamento>>> Criar([FromBody] FormaPagamentoCreateDto formaPagamentoDto)
        {
            var resultado = await _formaPagamentoService.CriarAsync(formaPagamentoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdFormaPagamento }, resultado);
        }

        /// <summary>
        /// Atualiza uma forma de pagamento existente
        /// </summary>
        /// <param name="id">ID da forma de pagamento</param>
        /// <param name="formaPagamentoDto">Dados atualizados</param>
        /// <returns>Forma de pagamento atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<FormaPagamento>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<FormaPagamento>>> Atualizar(int id, [FromBody] FormaPagamentoUpdateDto formaPagamentoDto)
        {
            var resultado = await _formaPagamentoService.AtualizarAsync(id, formaPagamentoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Ativa ou desativa uma forma de pagamento
        /// </summary>
        /// <param name="id">ID da forma de pagamento</param>
        /// <param name="ativo">true para ativar, false para desativar</param>
        /// <returns>Confirmação</returns>
        [HttpPut("{id}/ativar-desativar")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> AtivarDesativar(int id, [FromBody] bool ativo)
        {
            var resultado = await _formaPagamentoService.AtivarDesativarAsync(id, ativo);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta uma forma de pagamento
        /// </summary>
        /// <param name="id">ID da forma de pagamento</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _formaPagamentoService.DeletarAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }
    }
}

