using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;


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

        [Required]
        [StringLength(100)]
        public string Categoria { get; set; }


        [StringLength(500)]
        public string Descripcion { get; set; }

        public ICollection<Compra> Compras { get; set; }
    }
}
