using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoMestre.Models.DTOs.Common;
using PedidoMestre.Models.DTOs.Empresas;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Empresas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("1. Empresas")]
    public class EmpresasController : ControllerBase
    {
        private readonly IEmpresaService _empresaService;

        public EmpresasController(IEmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        /// <summary>
        /// Lista todas as empresas cadastradas
        /// </summary>
        /// <returns>Lista de empresas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Empresa>>), 200)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Empresa>>>> ObterTodos()
        {
            var resultado = await _empresaService.ObterTodosAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém uma empresa pelo ID
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns>Dados da empresa</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Empresa>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<Empresa>>> ObterPorId(int id)
        {
            var resultado = await _empresaService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém detalhes completos do perfil da empresa
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns>Detalhes completos da empresa</returns>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(ResponseModel<EmpresaDetalhesResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseModel<EmpresaDetalhesResponseDto>>> ObterDetalhes(int id)
        {
            var resultado = await _empresaService.ObterDetalhesAsync(id);
            if (!resultado.Status)
                return NotFound(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Cria uma nova empresa
        /// </summary>
        /// <param name="empresaDto">Dados básicos da empresa</param>
        /// <returns>Empresa criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Empresa>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Empresa>>> Criar([FromBody] EmpresaCreateDto empresaDto)
        {
            var resultado = await _empresaService.CriarAsync(empresaDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.IdEmpresa }, resultado);
        }

        /// <summary>
        /// Atualiza uma empresa existente
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <param name="empresa">Dados atualizados da empresa</param>
        /// <returns>Empresa atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Empresa>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Empresa>>> Atualizar(int id, [FromBody] Empresa empresa)
        {
            var resultado = await _empresaService.AtualizarAsync(id, empresa);
            return Ok(resultado);
        }

        /// <summary>
        /// Atualiza o perfil completo da empresa (foto, descrição, redes sociais, etc)
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <param name="empresaDto">Dados do perfil da empresa</param>
        /// <returns>Empresa atualizada</returns>
        [HttpPut("{id}/perfil")]
        [ProducesResponseType(typeof(ResponseModel<Empresa>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<Empresa>>> AtualizarPerfil(int id, [FromBody] EmpresaUpdateDto empresaDto)
        {
            var resultado = await _empresaService.AtualizarPerfilAsync(id, empresaDto);
            if (!resultado.Status)
                return BadRequest(resultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Deleta uma empresa
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseModel<bool>>> Deletar(int id)
        {
            var resultado = await _empresaService.DeletarAsync(id);
            return Ok(resultado);
        }
    }
}

