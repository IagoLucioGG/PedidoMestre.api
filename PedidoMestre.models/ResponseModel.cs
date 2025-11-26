namespace PedidoMestre.Models.Common
{
    public class ResponseModel<T>
    {
        public string Mensagem { get; set; } = string.Empty;
        public bool Status { get; set; }
        public T? Dados { get; set; }

        // Construtor de sucesso
        public ResponseModel(T dados, string mensagem = "Operação realizada com sucesso")
        {
            Status = true;
            Dados = dados;
            Mensagem = mensagem;
        }

        // Construtor de erro
        public ResponseModel(string mensagem)
        {
            Status = false;
            Mensagem = mensagem;
            Dados = default(T);
        }

        // Construtor padrão
        public ResponseModel()
        {
            Mensagem = string.Empty;
        }
    }
}

