using System.ComponentModel.DataAnnotations;

namespace PedidoMestre.Models.Produtos
{
    /// <summary>
    /// DTO simplificado para criação de vínculo Produto-Variação
    /// </summary>
    public class ProdutoVariacaoCreateDto
    {
        [Required(ErrorMessage = "IdProduto é obrigatório")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "IdVariacao é obrigatório")]
        public int IdVariacao { get; set; }
    }
}

