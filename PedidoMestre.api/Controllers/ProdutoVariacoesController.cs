using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Vínculos entre Produtos e Variações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("9. Produto-Variações")]
    public class ProdutoVariacoesController : ControllerBase
    {
        private readonly IProdutoVariacaoService _produtoVariacaoService;

        public ProdutoVariacoesController(IProdutoVariacaoService produtoVariacaoService)
        {
            _produtoVariacaoService = produtoVariacaoService;
        }

        /// <summary>
        /// Lista todos os vínculos entre produtos e variações
        /// </summary>
        /// <returns>Lista de vínculos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ProdutoVariacao>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<ProdutoVariacao>>>> ObterTodos()
        {
            var resultado = await _produtoVariacaoService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todas as variações de um produto
        /// </summary>
        /// <param name="idProduto">ID do produto</param>
        /// <returns>Lista de variações do produto</returns>
        [HttpGet("produto/{idProduto}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ProdutoVariacao>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<ProdutoVariacao>>>> ObterPorProduto(long idProduto)
        {
            var resultado = await _produtoVariacaoService.ObterPorProdutoIdAsync(idProduto);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todos os produtos que possuem uma variação específica
        /// </summary>
        /// <param name="idVariacao">ID da variação</param>
        /// <returns>Lista de produtos com a variação</returns>
        [HttpGet("variacao/{idVariacao}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ProdutoVariacao>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<ProdutoVariacao>>>> ObterPorVariacao(int idVariacao)
        {
            var resultado = await _produtoVariacaoService.ObterPorVariacaoIdAsync(idVariacao);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo vínculo entre produto e variação
        /// </summary>
        /// <param name="produtoVariacao">Dados do vínculo</param>
        /// <returns>Vínculo criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<ProdutoVariacao>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<ProdutoVariacao>>> Criar([FromBody] ProdutoVariacaoCreateDto produtoVariacaoDto)
        {
            var resultado = await _produtoVariacaoService.CriarAsync(produtoVariacaoDto);
            return Ok(resultado);
        }

        /// <summary>
        /// Remove um vínculo entre produto e variação
        /// </summary>
        /// <param name="idProduto">ID do produto</param>
        /// <param name="idVariacao">ID da variação</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{idProduto}/{idVariacao}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(long idProduto, int idVariacao)
        {
            var resultado = await _produtoVariacaoService.DeletarAsync(idProduto, idVariacao);
            return Ok(resultado);
        }
    }
}

