using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Direccion { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Envio> Envios { get; set; }
    }
}