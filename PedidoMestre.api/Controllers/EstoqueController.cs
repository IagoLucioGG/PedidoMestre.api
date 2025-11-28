using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Estoque;
using PedidoMestre.Models.Estoque;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Estoque
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("13. Estoque")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;

        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        /// <summary>
        /// Lista o estoque de todos os produtos de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de estoque de produtos</returns>
        [HttpGet("produtos/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<EstoqueProdutoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<EstoqueProdutoResponseDto>>>> ObterEstoqueProdutos(int idLoja)
        {
            var resultado = await _estoqueService.ObterEstoqueProdutosAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém o estoque de um produto específico
        /// </summary>
        /// <param name="idProduto">ID do produto</param>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Estoque do produto</returns>
        [HttpGet("produto/{idProduto}/loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<EstoqueProdutoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<EstoqueProdutoResponseDto>>> ObterEstoqueProduto(long idProduto, int idLoja)
        {
            var resultado = await _estoqueService.ObterEstoqueProdutoAsync(idProduto, idLoja);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Atualiza o estoque de um produto
        /// </summary>
        /// <param name="idProduto">ID do produto</param>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="estoqueDto">Dados do estoque</param>
        /// <returns>Confirmação de atualização</returns>
        [HttpPut("produto/{idProduto}/loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> AtualizarEstoqueProduto(long idProduto, int idLoja, [FromBody] EstoqueUpdateDto estoqueDto)
        {
            var resultado = await _estoqueService.AtualizarEstoqueProdutoAsync(idProduto, idLoja, estoqueDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista o estoque de todas as opções (variações) de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de estoque de opções</returns>
        [HttpGet("opcoes/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<EstoqueOpcaoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<EstoqueOpcaoResponseDto>>>> ObterEstoqueOpcoes(int idLoja)
        {
            var resultado = await _estoqueService.ObterEstoqueOpcoesAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todos os insumos de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de insumos</returns>
        [HttpGet("insumos/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<InsumoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<InsumoResponseDto>>>> ObterInsumos(int idLoja)
        {
            var resultado = await _estoqueService.ObterInsumosAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um insumo específico
        /// </summary>
        /// <param name="id">ID do insumo</param>
        /// <returns>Dados do insumo</returns>
        [HttpGet("insumo/{id}")]
        [ProducesResponseType(typeof(ResponseModel<InsumoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<InsumoResponseDto>>> ObterInsumo(long id)
        {
            var resultado = await _estoqueService.ObterInsumoAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo insumo
        /// </summary>
        /// <param name="insumoDto">Dados do insumo</param>
        /// <returns>Insumo criado</returns>
        [HttpPost("insumo")]
        [ProducesResponseType(typeof(ResponseModel<Insumo>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Insumo>>> CriarInsumo([FromBody] InsumoCreateDto insumoDto)
        {
            var resultado = await _estoqueService.CriarInsumoAsync(insumoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return CreatedAtAction(nameof(ObterInsumo), new { id = resultado.Dados?.IdInsumo }, resultado);
        }

        /// <summary>
        /// Atualiza um insumo existente
        /// </summary>
        /// <param name="id">ID do insumo</param>
        /// <param name="insumoDto">Dados atualizados</param>
        /// <returns>Insumo atualizado</returns>
        [HttpPut("insumo/{id}")]
        [ProducesResponseType(typeof(ResponseModel<Insumo>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Insumo>>> AtualizarInsumo(long id, [FromBody] InsumoUpdateDto insumoDto)
        {
            var resultado = await _estoqueService.AtualizarInsumoAsync(id, insumoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um insumo
        /// </summary>
        /// <param name="id">ID do insumo</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("insumo/{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> DeletarInsumo(long id)
        {
            var resultado = await _estoqueService.DeletarInsumoAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista as movimentações de estoque de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <param name="idProduto">ID do produto (opcional, para filtrar)</param>
        /// <returns>Lista de movimentações</returns>
        [HttpGet("movimentacoes/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<MovimentacaoEstoqueResponseDto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<MovimentacaoEstoqueResponseDto>>>> ObterMovimentacoes(int idLoja, [FromQuery] long? idProduto = null)
        {
            var resultado = await _estoqueService.ObterMovimentacoesAsync(idLoja, idProduto);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria uma nova movimentação de estoque
        /// </summary>
        /// <param name="movimentacaoDto">Dados da movimentação</param>
        /// <returns>Movimentação criada</returns>
        [HttpPost("movimentacao")]
        [ProducesResponseType(typeof(ResponseModel<MovimentacaoEstoque>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<MovimentacaoEstoque>>> CriarMovimentacao([FromBody] MovimentacaoEstoqueCreateDto movimentacaoDto)
        {
            var resultado = await _estoqueService.CriarMovimentacaoAsync(movimentacaoDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return CreatedAtAction(nameof(ObterMovimentacoes), new { idLoja = movimentacaoDto.IdLoja }, resultado);
        }

        /// <summary>
        /// Obtém a ficha técnica de um produto
        /// </summary>
        /// <param name="idProduto">ID do produto</param>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Ficha técnica do produto</returns>
        [HttpGet("ficha-tecnica/produto/{idProduto}/loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<FichaTecnicaResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<FichaTecnicaResponseDto>>> ObterFichaTecnica(long idProduto, int idLoja)
        {
            var resultado = await _estoqueService.ObterFichaTecnicaAsync(idProduto, idLoja);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }
    }
}

