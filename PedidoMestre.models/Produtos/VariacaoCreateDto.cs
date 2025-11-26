using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Produtos
{
    /// <summary>
    /// DTO simplificado para criação de Variação
    /// </summary>
    public class VariacaoCreateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        public decimal? PrecoExtra { get; set; }
    }
}

