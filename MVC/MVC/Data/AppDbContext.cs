using MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) {}

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<VideoJuego> VideoJuegos { get; set; }

        public DbSet<Compra> Compras { get; set; }

        
    }
}
