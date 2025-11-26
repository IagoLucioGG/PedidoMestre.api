using PedidoMestre.Models.Common;

namespace PedidoMestre.Services.Interfaces
{
    public interface ICnpjService
    {
        Task<ResponseModel<CnpjInfo>> ValidarCnpjAsync(string cnpj);
    }

    public class CnpjInfo
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Cep { get; set; }
        public bool Valido { get; set; }
    }
}

