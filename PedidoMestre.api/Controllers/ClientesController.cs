using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Clientes;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Clientes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("4. Clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Lista todos os clientes cadastrados
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Cliente>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Cliente>>>> ObterTodos()
        {
            var resultado = await _clienteService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém um cliente pelo ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Dados do cliente</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Cliente>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Cliente>>> ObterPorId(long id)
        {
            var resultado = await _clienteService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="clienteDto">Dados básicos do cliente</param>
        /// <returns>Cliente criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Cliente>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Cliente>>> Criar([FromBody] ClienteCreateDto clienteDto)
        {
            var resultado = await _clienteService.CriarAsync(clienteDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdCliente }, resultado);
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="cliente">Dados atualizados do cliente</param>
        /// <returns>Cliente atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Cliente>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Cliente>>> Atualizar(long id, [FromBody] Cliente cliente)
        {
            var resultado = await _clienteService.AtualizarAsync(id, cliente);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(long id)
        {
            var resultado = await _clienteService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

