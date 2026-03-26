using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class Usuario
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public byte[] Password { get; set; }


        [Required]
        public string Salt { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Compra> Compras { get; set; }
    }
}