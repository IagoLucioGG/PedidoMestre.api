using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Produtos
{
    /// <summary>
    /// DTO simplificado para criação de Adicional
    /// </summary>
    public class AdicionalCreateDto
    {
        [Required(ErrorMessage = "IdLoja é obrigatório")]
        public int IdLoja { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preco é obrigatório")]
        public decimal Preco { get; set; }
    }
}

