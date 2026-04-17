using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class VideoJuego
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        public string? imagen { get; set; }

        [Required]
        [Range(0, 21, ErrorMessage = "Edad inválida")]
        public int EdadMinima { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }

        public int? PromocionId { get; set; }

        [ForeignKey("PromocionId")]
        public Promocion? Promocion { get; set; }
    }
}