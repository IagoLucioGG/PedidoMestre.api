using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Empresas
{
    /// <summary>
    /// DTO simplificado para criação de Empresa
    /// </summary>
    public class EmpresaCreateDto
    {
        [Required(ErrorMessage = "Nome Fantasia é obrigatório")]
        [MaxLength(255)]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [MaxLength(18)]
        public string Cnpj { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        public decimal? TaxaPorKm { get; set; }
    }
}

