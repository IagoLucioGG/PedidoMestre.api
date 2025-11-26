using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Categorias
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("5. Categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Lista todas as categorias cadastradas
        /// </summary>
        /// <returns>Lista de categorias</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Categoria>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Categoria>>>> ObterTodos()
        {
            var resultado = await _categoriaService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém uma categoria pelo ID
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Dados da categoria</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Categoria>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Categoria>>> ObterPorId(int id)
        {
            var resultado = await _categoriaService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todas as categorias de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de categorias da loja</returns>
        [HttpGet("loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Categoria>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Categoria>>>> ObterPorLoja(int idLoja)
        {
            var resultado = await _categoriaService.ObterPorLojaIdAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        /// <param name="categoria">Dados da categoria</param>
        /// <returns>Categoria criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Categoria>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Categoria>>> Criar([FromBody] CategoriaCreateDto categoriaDto)
        {
            var resultado = await _categoriaService.CriarAsync(categoriaDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdCategoria }, resultado);
        }

        /// <summary>
        /// Atualiza uma categoria existente
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <param name="categoria">Dados atualizados da categoria</param>
        /// <returns>Categoria atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Categoria>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Categoria>>> Atualizar(int id, [FromBody] Categoria categoria)
        {
            var resultado = await _categoriaService.AtualizarAsync(id, categoria);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta uma categoria
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _categoriaService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

