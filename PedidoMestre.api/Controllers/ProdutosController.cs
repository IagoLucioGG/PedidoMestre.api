using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Produtos;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Produtos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("7. Produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        /// <summary>
        /// Lista todos os produtos cadastrados
        /// </summary>
        /// <returns>Lista de produtos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Produto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Produto>>>> ObterTodos()
        {
            var resultado = await _produtoService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um produto pelo ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Dados do produto</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Produto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Produto>>> ObterPorId(long id)
        {
            var resultado = await _produtoService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todos os produtos de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de produtos da loja</returns>
        [HttpGet("loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Produto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Produto>>>> ObterPorLoja(int idLoja)
        {
            var resultado = await _produtoService.ObterPorLojaIdAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todos os produtos de uma categoria
        /// </summary>
        /// <param name="idCategoria">ID da categoria</param>
        /// <returns>Lista de produtos da categoria</returns>
        [HttpGet("categoria/{idCategoria}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Produto>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Produto>>>> ObterPorCategoria(int idCategoria)
        {
            var resultado = await _produtoService.ObterPorCategoriaIdAsync(idCategoria);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <param name="produto">Dados do produto</param>
        /// <returns>Produto criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Produto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Produto>>> Criar([FromBody] ProdutoCreateDto produtoDto)
        {
            var resultado = await _produtoService.CriarAsync(produtoDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdProduto }, resultado);
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <param name="produto">Dados atualizados do produto</param>
        /// <returns>Produto atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Produto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Produto>>> Atualizar(long id, [FromBody] Produto produto)
        {
            var resultado = await _produtoService.AtualizarAsync(id, produto);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um produto
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(long id)
        {
            var resultado = await _produtoService.DeletarAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Marca um produto como em falta ou remove da lista de falta
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <param name="emFalta">true para marcar como em falta, false para remover</param>
        /// <returns>Confirmação</returns>
        [HttpPut("{id}/em-falta")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> MarcarEmFalta(long id, [FromBody] bool emFalta)
        {
            var resultado = await _produtoService.MarcarEmFaltaAsync(id, emFalta);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Oculta ou exibe um produto
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <param name="oculto">true para ocultar, false para exibir</param>
        /// <returns>Confirmação</returns>
        [HttpPut("{id}/ocultar")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<bool>>> Ocultar(long id, [FromBody] bool oculto)
        {
            var resultado = await _produtoService.OcultarAsync(id, oculto);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Duplica um produto existente
        /// </summary>
        /// <param name="id">ID do produto a ser duplicado</param>
        /// <returns>Produto duplicado</returns>
        [HttpPost("{id}/duplicar")]
        [ProducesResponseType(typeof(ResponseModel<Produto>), 201)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Produto>>> Duplicar(long id)
        {
            var resultado = await _produtoService.DuplicarAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdProduto }, resultado);
        }
    }
}

