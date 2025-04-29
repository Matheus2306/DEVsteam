namespace APIDevSteam1.Models
{
    public class Carrinho
    {
        public Guid CarrinhoId { get; set; }
        public string CarrinhoNome { get; set; }
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public bool Finalizado { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public decimal valorTotal { get; set; }
    }
}
