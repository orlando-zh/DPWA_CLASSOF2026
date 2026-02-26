using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace MVC.Models
{
    public class Compra
    {
        [Key]
        public int id { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Required]
        public int videoJuegoId { get; set; }

        [ForeignKey("videoJuegoId")]
        public VideoJuego VideoJuego { get; set; }
    }
}
