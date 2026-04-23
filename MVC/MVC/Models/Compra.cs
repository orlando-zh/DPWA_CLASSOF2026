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

        public int UsuarioId { get; set; } 

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

    }
}