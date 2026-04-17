using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(250)]
        public string? Descripcion { get; set; }

        // Relación: Una categoría tiene muchos videojuegos
        public ICollection<VideoJuego>? VideoJuegos { get; set; }
    }
}