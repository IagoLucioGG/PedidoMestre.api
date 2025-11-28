using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Produtos;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Adicionais
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("9. Adicionais")]
    public class AdicionaisController : ControllerBase
    {
        private readonly IAdicionalService _adicionalService;

        public AdicionaisController(IAdicionalService adicionalService)
        {
            _adicionalService = adicionalService;
        }

        /// <summary>
        /// Lista todos os adicionais cadastrados
        /// </summary>
        /// <returns>Lista de adicionais</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Adicional>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Adicional>>>> ObterTodos()
        {
            var resultado = await _adicionalService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um adicional pelo ID
        /// </summary>
        /// <param name="id">ID do adicional</param>
        /// <returns>Dados do adicional</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Adicional>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Adicional>>> ObterPorId(int id)
        {
            var resultado = await _adicionalService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Lista todos os adicionais de uma loja
        /// </summary>
        /// <param name="idLoja">ID da loja</param>
        /// <returns>Lista de adicionais da loja</returns>
        [HttpGet("loja/{idLoja}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Adicional>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Adicional>>>> ObterPorLoja(int idLoja)
        {
            var resultado = await _adicionalService.ObterPorLojaIdAsync(idLoja);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo adicional
        /// </summary>
        /// <param name="adicional">Dados do adicional</param>
        /// <returns>Adicional criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Adicional>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Adicional>>> Criar([FromBody] AdicionalCreateDto adicionalDto)
        {
            var resultado = await _adicionalService.CriarAsync(adicionalDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdAdicional }, resultado);
        }

        /// <summary>
        /// Atualiza um adicional existente
        /// </summary>
        /// <param name="id">ID do adicional</param>
        /// <param name="adicional">Dados atualizados do adicional</param>
        /// <returns>Adicional atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Adicional>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Adicional>>> Atualizar(int id, [FromBody] Adicional adicional)
        {
            var resultado = await _adicionalService.AtualizarAsync(id, adicional);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um adicional
        /// </summary>
        /// <param name="id">ID do adicional</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _adicionalService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

