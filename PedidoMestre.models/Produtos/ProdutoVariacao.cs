namespace PedidoMestre.Models.Produtos
{
    public class ProdutoVariacao
    {
        public long IdProduto { get; set; }

        public int IdVariacao { get; set; }

        // Relacionamento: ProdutoVariacao pertence a um produto
        public Produto Produto { get; set; }

        // Relacionamento: ProdutoVariacao pertence a uma variação
        public Variacao Variacao { get; set; }
    }
}

