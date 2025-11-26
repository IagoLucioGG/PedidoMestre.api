using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Geral;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Bairros
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("4. Clientes")]
    public class BairrosController : ControllerBase
    {
        private readonly IBairroService _bairroService;

        public BairrosController(IBairroService bairroService)
        {
            _bairroService = bairroService;
        }

        /// <summary>
        /// Busca bairros de uma cidade específica (via API IBGE)
        /// </summary>
        /// <param name="cidade">Nome da cidade</param>
        /// <param name="uf">UF (sigla do estado)</param>
        /// <returns>Lista de bairros encontrados</returns>
        [HttpGet("cidade/{cidade}/{uf}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bairro>>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Bairro>>>> BuscarPorCidade(string cidade, string uf)
        {
            var resultado = await _bairroService.BuscarBairrosPorCidadeAsync(cidade, uf);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria bairros automaticamente para uma loja baseado na cidade/UF
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="cidade">Nome da cidade</param>
        /// <param name="uf">UF (sigla do estado)</param>
        /// <param name="latitudeLoja">Latitude da loja (opcional)</param>
        /// <param name="longitudeLoja">Longitude da loja (opcional)</param>
        /// <param name="taxaPorKm">Taxa por KM (opcional, usa padrão da empresa ou 7,50)</param>
        /// <returns>Lista de bairros criados</returns>
        [HttpPost("loja/{idLoja}/criar-automatico")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bairro>>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Bairro>>>> CriarAutomaticamente(
            int idLoja,
            [FromQuery] string cidade,
            [FromQuery] string uf,
            [FromQuery] decimal? latitudeLoja = null,
            [FromQuery] decimal? longitudeLoja = null,
            [FromQuery] decimal? taxaPorKm = null)
        {
            var resultado = await _bairroService.CriarBairrosAutomaticamenteAsync(
                idLoja, cidade, uf, latitudeLoja, longitudeLoja, taxaPorKm);
            return Ok(resultado);
        }

        /// <summary>
        /// Atualiza a taxa de entrega de um bairro específico
        /// </summary>
        /// <param name="idBairro">ID do bairro</param>
        /// <param name="novaTaxa">Nova taxa de entrega</param>
        /// <returns>Bairro atualizado</returns>
        [HttpPut("{idBairro}/taxa")]
        [ProducesResponseType(typeof(ResponseModel<Bairro>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Bairro>>> AtualizarTaxa(int idBairro, [FromBody] decimal novaTaxa)
        {
            var resultado = await _bairroService.AtualizarTaxaAsync(idBairro, novaTaxa);
            return Ok(resultado);
        }
    }
}

