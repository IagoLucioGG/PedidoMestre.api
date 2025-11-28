using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Pedidos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de KDS (Kitchen Display System - Comandas Cozinha)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("12. KDS - Comandas Cozinha")]
    public class KdsController : ControllerBase
    {
        private readonly IKdsService _kdsService;

        public KdsController(IKdsService kdsService)
        {
            _kdsService = kdsService;
        }

        /// <summary>
        /// Obtém resumo do KDS (quantidade em preparação e prontos)
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Resumo do KDS</returns>
        [HttpGet("resumo")]
        [ProducesResponseType(typeof(ResponseModel<KdsResumoDto>), 200)]
        public async Task<ActionResult<ResponseModel<KdsResumoDto>>> ObterResumo([FromQuery] int idLoja)
        {
            var resultado = await _kdsService.ObterResumoAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista pedidos em preparação
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de pedidos em preparação</returns>
        [HttpGet("pedidos-preparacao")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<KdsPedidoResumoDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<KdsPedidoResumoDto>>>> ObterPedidosEmPreparacao([FromQuery] int idLoja)
        {
            var resultado = await _kdsService.ObterPedidosEmPreparacaoAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista pedidos prontos para entrega
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de pedidos prontos</returns>
        [HttpGet("pedidos-prontos")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<KdsPedidoResumoDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<KdsPedidoResumoDto>>>> ObterPedidosProntos([FromQuery] int idLoja)
        {
            var resultado = await _kdsService.ObterPedidosProntosAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém detalhes completos de um pedido para a cozinha
        /// </summary>
        /// <param name="idPedido">ID do pedido</param>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Detalhes do pedido</returns>
        [HttpGet("pedido/{idPedido}/detalhes")]
        [ProducesResponseType(typeof(ResponseModel<KdsPedidoDetalhesDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<KdsPedidoDetalhesDto>>> ObterDetalhes(long idPedido, [FromQuery] int idLoja)
        {
            var resultado = await _kdsService.ObterDetalhesAsync(idPedido, idLoja);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Marca pedido como pronto para entrega
        /// </summary>
        /// <param name="idPedido">ID do pedido</param>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Confirmação</returns>
        [HttpPut("pedido/{idPedido}/pronto-entrega")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> MarcarProntoParaEntrega(long idPedido, [FromQuery] int idLoja)
        {
            var resultado = await _kdsService.MarcarProntoParaEntregaAsync(idPedido, idLoja);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Retorna pedido para preparação (mantendo tempo anterior)
        /// </summary>
        /// <param name="idPedido">ID do pedido</param>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Confirmação</returns>
        [HttpPut("pedido/{idPedido}/em-preparacao")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> RetornarParaPreparacao(long idPedido, [FromQuery] int idLoja)
        {
            var resultado = await _kdsService.RetornarParaPreparacaoAsync(idPedido, idLoja);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Inicia preparação de um pedido
        /// </summary>
        /// <param name="idPedido">ID do pedido</param>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Confirmação</returns>
        [HttpPut("pedido/{idPedido}/iniciar-preparacao")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> IniciarPreparacao(long idPedido, [FromQuery] int idLoja)
        {
            var resultado = await _kdsService.IniciarPreparacaoAsync(idPedido, idLoja);
            
            if (!resultado.Status)
                return NotFound(resultado);

            return Ok(resultado);
        }
    }
}

