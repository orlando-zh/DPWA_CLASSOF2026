using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class EstadoEnvio
    {
        [Key]
        public int EstadoId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreEstado { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }

        public ICollection<Envio> Envios { get; set; }
    }
}