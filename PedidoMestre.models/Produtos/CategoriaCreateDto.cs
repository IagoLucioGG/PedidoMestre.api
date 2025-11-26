using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Produtos
{
    /// <summary>
    /// DTO simplificado para criação de Categoria
    /// </summary>
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "IdLoja é obrigatório")]
        public int IdLoja { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ordem é obrigatória")]
        public int Ordem { get; set; }
    }
}

