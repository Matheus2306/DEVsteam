namespace APIDevSteam1.Models
{
    public class JogoCategoria
    {
        public Guid JogoCategoriaID { get; set; }
        public Guid JogoID { get; set; }
        public Jogo? Jogo { get; set; }
        public Guid CategoriaID { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
