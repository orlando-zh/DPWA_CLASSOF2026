using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class EstadoPago
    {
        [Key]
        public int EstadoPagoId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreEstado { get; set; }

        public ICollection<Envio>? Envios { get; set; }
    }
}