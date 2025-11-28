using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Desempenho;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para análise de Desempenho/Dashboard
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("15. Desempenho")]
    public class DesempenhoController : ControllerBase
    {
        private readonly IDesempenhoService _desempenhoService;

        public DesempenhoController(IDesempenhoService desempenhoService)
        {
            _desempenhoService = desempenhoService;
        }

        /// <summary>
        /// Obtém métricas gerais de desempenho de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <returns>Métricas gerais</returns>
        [HttpGet("metricas/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<DesempenhoMetricasDto>), 200)]
        public async Task<ActionResult<ResponseModel<DesempenhoMetricasDto>>> ObterMetricas(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _desempenhoService.ObterMetricasAsync(idLoja, dataInicio, dataFim);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém análise de vendas por período
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <returns>Análise de vendas</returns>
        [HttpGet("vendas/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<DesempenhoVendasDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<DesempenhoVendasDto>>>> ObterVendas(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _desempenhoService.ObterVendasAsync(idLoja, dataInicio, dataFim);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém os produtos mais vendidos
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <param name="top">Quantidade de produtos a retornar (padrão: 10)</param>
        /// <returns>Lista de produtos mais vendidos</returns>
        [HttpGet("produtos-mais-vendidos/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ProdutoMaisVendidoDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<ProdutoMaisVendidoDto>>>> ObterProdutosMaisVendidos(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null, [FromQuery] int top = 10)
        {
            var resultado = await _desempenhoService.ObterProdutosMaisVendidosAsync(idLoja, dataInicio, dataFim, top);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém o tempo médio de preparo
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <returns>Tempo médio de preparo</returns>
        [HttpGet("tempo-medio-preparo/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<TimeSpan>), 200)]
        public async Task<ActionResult<ResponseModel<TimeSpan>>> ObterTempoMedioPreparo(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _desempenhoService.ObterTempoMedioPreparoAsync(idLoja, dataInicio, dataFim);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém o tempo médio de entrega
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <returns>Tempo médio de entrega</returns>
        [HttpGet("tempo-medio-entrega/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<TimeSpan>), 200)]
        public async Task<ActionResult<ResponseModel<TimeSpan>>> ObterTempoMedioEntrega(int idLoja, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _desempenhoService.ObterTempoMedioEntregaAsync(idLoja, dataInicio, dataFim);
            return Ok(resultado);
        }
    }
}

