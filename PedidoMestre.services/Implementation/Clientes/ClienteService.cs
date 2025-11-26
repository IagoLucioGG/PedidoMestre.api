using Microsoft.EntityFrameworkCore;
using PedidoMestre.Data;
using PedidoMestre.Models.Common;
using PedidoMestre.Models.Clientes;
using PedidoMestre.Services.Interfaces;

namespace PedidoMestre.Services.Implementation.Clientes
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<Cliente>>> ObterTodosAsync()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Enderecos)
                    .ThenInclude(e => e.Bairro)
                .Select(c => new Cliente
                {
                    IdCliente = c.IdCliente,
                    Nome = c.Nome,
                    Telefone = c.Telefone,
                    Email = c.Email,
                    SenhaHash = c.SenhaHash,
                    Enderecos = c.Enderecos
                })
                .ToListAsync();

            return new ResponseModel<IEnumerable<Cliente>>(clientes, "Clientes obtidos com sucesso");
        }

        public async Task<ResponseModel<Cliente>> ObterPorIdAsync(long id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Enderecos)
                    .ThenInclude(e => e.Bairro)
                .Select(c => new Cliente
                {
                    IdCliente = c.IdCliente,
                    Nome = c.Nome,
                    Telefone = c.Telefone,
                    Email = c.Email,
                    SenhaHash = c.SenhaHash,
                    Enderecos = c.Enderecos
                })
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");
            }

            return new ResponseModel<Cliente>(cliente, "Cliente obtido com sucesso");
        }

        public async Task<ResponseModel<Cliente>> CriarAsync(ClienteCreateDto clienteDto)
        {
            if (clienteDto == null)
            {
                throw new ArgumentNullException(nameof(clienteDto), "Dados do cliente não podem ser nulos");
            }

            // Verificar se já existe cliente com o mesmo email (quando email não for nulo)
            if (!string.IsNullOrEmpty(clienteDto.Email))
            {
                var emailExistente = await _context.Clientes
                    .AnyAsync(c => c.Email == clienteDto.Email);
                if (emailExistente)
                {
                    throw new ArgumentException($"Já existe um cliente cadastrado com o email {clienteDto.Email}");
                }
            }

            // Criar objeto Cliente a partir do DTO
            var cliente = new Cliente
            {
                Nome = clienteDto.Nome,
                Telefone = clienteDto.Telefone,
                Email = clienteDto.Email,
                SenhaHash = !string.IsNullOrEmpty(clienteDto.Senha) 
                    ? AuthService.HashSenha(clienteDto.Senha) 
                    : null
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // Carregar relacionamentos após salvar
            await _context.Entry(cliente)
                .Collection(c => c.Enderecos)
                .LoadAsync();

            return new ResponseModel<Cliente>(cliente, "Cliente criado com sucesso");
        }

        public async Task<ResponseModel<Cliente>> AtualizarAsync(long id, Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "Cliente não pode ser nulo");
            }

            var clienteExistente = await _context.Clientes.FindAsync(id);

            if (clienteExistente == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");
            }

            // Verificar se o email foi alterado e se já existe outro cadastro com esse email
            if (!string.IsNullOrEmpty(cliente.Email) && cliente.Email != clienteExistente.Email)
            {
                var emailExistente = await _context.Clientes
                    .AnyAsync(c => c.Email == cliente.Email && c.IdCliente != id);
                if (emailExistente)
                {
                    throw new ArgumentException($"Já existe outro cliente cadastrado com o email {cliente.Email}");
                }
            }

            clienteExistente.Nome = cliente.Nome;
            clienteExistente.Telefone = cliente.Telefone;
            clienteExistente.Email = cliente.Email;
            
            // Se a senha foi informada, fazer hash antes de atualizar
            if (!string.IsNullOrEmpty(cliente.SenhaHash))
            {
                clienteExistente.SenhaHash = AuthService.HashSenha(cliente.SenhaHash);
            }

            await _context.SaveChangesAsync();

            // Carregar relacionamentos após atualizar
            await _context.Entry(clienteExistente)
                .Collection(c => c.Enderecos)
                .LoadAsync();

            return new ResponseModel<Cliente>(clienteExistente, "Cliente atualizado com sucesso");
        }

        public async Task<ResponseModel<bool>> DeletarAsync(long id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Enderecos)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");
            }

            // Verificar se o cliente possui endereços vinculados
            if (cliente.Enderecos.Any())
            {
                throw new InvalidOperationException($"Não é possível deletar o cliente pois ele possui {cliente.Enderecos.Count} endereço(s) vinculado(s)");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>(true, "Cliente deletado com sucesso");
        }
    }
}

