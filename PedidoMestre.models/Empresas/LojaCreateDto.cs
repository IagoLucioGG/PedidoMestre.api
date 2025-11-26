using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Empresas
{
    /// <summary>
    /// DTO simplificado para criação de Loja
    /// </summary>
    public class LojaCreateDto
    {
        [Required(ErrorMessage = "IdEmpresa é obrigatório")]
        public int IdEmpresa { get; set; }

        [Required(ErrorMessage = "Endereço é obrigatório")]
        [MaxLength(255)]
        public string Endereco { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        public string? ConfigDelivery { get; set; }
    }
}

