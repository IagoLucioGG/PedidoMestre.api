using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Pedidos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Pedidos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("11. Pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Obtém resumo superior da tela de gestão de pedidos (quantidade por status, tempos médios)
        /// </summary>
        /// <param name="idLoja">ID da loja (opcional)</param>
        /// <param name="data">Data para consulta (opcional, padrão: hoje)</param>
        /// <returns>Resumo com estatísticas dos pedidos</returns>
        [HttpGet("resumo")]
        [ProducesResponseType(typeof(ResponseModel<PedidoResumoSuperiorDto>), 200)]
        public async Task<ActionResult<ResponseModel<PedidoResumoSuperiorDto>>> ObterResumoSuperior(
            [FromQuery] int? idLoja = null,
            [FromQuery] DateTime? data = null)
        {
            var resultado = await _pedidoService.ObterResumoSuperiorAsync(idLoja, data);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todos os pedidos com filtros opcionais
        /// </summary>
        /// <param name="idLoja">ID da loja (opcional)</param>
        /// <param name="status">Status do pedido (opcional)</param>
        /// <param name="dataInicio">Data inicial para filtro (opcional)</param>
        /// <param name="dataFim">Data final para filtro (opcional)</param>
        /// <returns>Lista de pedidos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<PedidoResumoDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<PedidoResumoDto>>>> ObterTodos(
            [FromQuery] int? idLoja = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _pedidoService.ObterTodosAsync(idLoja, status, dataInicio, dataFim);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista pedidos do dia atual
        /// </summary>
        /// <param name="idLoja">ID da loja (opcional)</param>
        /// <returns>Lista de pedidos do dia</returns>
        [HttpGet("hoje")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<PedidoResumoDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<PedidoResumoDto>>>> ObterPedidosHoje(
            [FromQuery] int? idLoja = null)
        {
            var resultado = await _pedidoService.ObterPedidosHojeAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém detalhes completos de um pedido
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <returns>Detalhes completos do pedido</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<PedidoDetalhesResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<PedidoDetalhesResponseDto>>> ObterPorId(long id)
        {
            var resultado = await _pedidoService.ObterPorIdAsync(id);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Obtém histórico de mudanças de status de um pedido
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <returns>Histórico de status</returns>
        [HttpGet("{id}/historico")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<PedidoHistoricoDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<PedidoHistoricoDto>>>> ObterHistorico(long id)
        {
            var resultado = await _pedidoService.ObterHistoricoAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        /// <param name="pedidoDto">Dados do pedido</param>
        /// <returns>Pedido criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<PedidoMestre.Models.Pedidos.Pedido>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<PedidoMestre.Models.Pedidos.Pedido>>> Criar([FromBody] PedidoCreateDto pedidoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _pedidoService.CriarAsync(pedidoDto);
            
            if (!resultado.Status)
                return BadRequest(resultado);

            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdPedido }, resultado);
        }

        /// <summary>
        /// Atualiza o status de um pedido
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <param name="statusDto">Novo status e observações</param>
        /// <returns>Pedido atualizado</returns>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ResponseModel<PedidoMestre.Models.Pedidos.Pedido>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<PedidoMestre.Models.Pedidos.Pedido>>> AtualizarStatus(
            long id,
            [FromBody] PedidoStatusUpdateDto statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _pedidoService.AtualizarStatusAsync(id, statusDto);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Finaliza um pedido (marca como concluído)
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <returns>Pedido finalizado</returns>
        [HttpPut("{id}/finalizar")]
        [ProducesResponseType(typeof(ResponseModel<PedidoMestre.Models.Pedidos.Pedido>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<PedidoMestre.Models.Pedidos.Pedido>>> FinalizarPedido(long id)
        {
            var resultado = await _pedidoService.FinalizarPedidoAsync(id);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Marca pedido como "Saiu para Entrega"
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <param name="idEntregador">ID do entregador (opcional)</param>
        /// <returns>Pedido atualizado</returns>
        [HttpPut("{id}/saiu-entrega")]
        [ProducesResponseType(typeof(ResponseModel<PedidoMestre.Models.Pedidos.Pedido>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<PedidoMestre.Models.Pedidos.Pedido>>> MarcarSaiuEntrega(
            long id,
            [FromQuery] int? idEntregador = null)
        {
            var resultado = await _pedidoService.MarcarSaiuEntregaAsync(id, idEntregador);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Cancela um pedido
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <param name="motivo">Motivo do cancelamento (opcional)</param>
        /// <returns>Pedido cancelado</returns>
        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(typeof(ResponseModel<PedidoMestre.Models.Pedidos.Pedido>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<PedidoMestre.Models.Pedidos.Pedido>>> CancelarPedido(
            long id,
            [FromQuery] string? motivo = null)
        {
            var resultado = await _pedidoService.CancelarPedidoAsync(id, motivo);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Gera número único para pedido
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Número do pedido gerado</returns>
        [HttpGet("gerar-numero")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        public async Task<ActionResult<ResponseModel<string>>> GerarNumeroPedido([FromQuery] int idLoja)
        {
            var resultado = await _pedidoService.GerarNumeroPedidoAsync(idLoja);
            return Ok(resultado);
        }
    }
}

