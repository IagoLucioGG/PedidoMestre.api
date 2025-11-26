using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Produtos
{
    /// <summary>
    /// DTO simplificado para criação de Produto
    /// </summary>
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "IdCategoria é obrigatório")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "IdLoja é obrigatório")]
        public int IdLoja { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "PrecoBase é obrigatório")]
        public decimal PrecoBase { get; set; }

        public bool Ativo { get; set; } = true;

        public int? TempoPreparoMin { get; set; }
    }
}

