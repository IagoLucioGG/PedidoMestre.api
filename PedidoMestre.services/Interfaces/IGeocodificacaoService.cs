using PedidoMestre.Models.Common;

namespace PedidoMestre.Services.Interfaces
{
    public interface IGeocodificacaoService
    {
        Task<ResponseModel<Coordenadas>> ObterCoordenadasPorEnderecoAsync(string endereco, string cidade, string? cep = null);
        Task<ResponseModel<Coordenadas>> ObterCoordenadasPorCepAsync(string cep);
        Task<ResponseModel<EnderecoCompleto>> ObterEnderecoCompletoAsync(string endereco);
    }

    public class Coordenadas
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? EnderecoCompleto { get; set; }
    }

    public class EnderecoCompleto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? EnderecoFormatado { get; set; }
    }
}

