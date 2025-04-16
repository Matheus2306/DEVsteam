using APIDevSteam1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIDevSteam1.Data
{
    public class APIContext : IdentityDbContext<Usuario>
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {
        }

        //Dbset
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<JogoCategoria> JogosCategorias { get; set; }
        public DbSet<JogoMidia> jogoMidias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // tabelas
            modelBuilder.Entity<Jogo>().ToTable("Jogos");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<JogoCategoria>().ToTable("JogosCategorias");
            modelBuilder.Entity<JogoMidia>().ToTable("JogosMidias");
        }
    }
}
