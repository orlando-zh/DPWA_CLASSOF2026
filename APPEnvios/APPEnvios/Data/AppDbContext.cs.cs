using Microsoft.EntityFrameworkCore;
using APPEnvios.Models;

namespace APPEnvios.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) {}
        public DbSet<Envio> Envios { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Destinatario> Destinatarios { get; set; }

        public DbSet<EstadoEnvio> EstadosEnvio { get; set; }

        public DbSet<Paquete> Paquetes { get; set; }
    }
}
