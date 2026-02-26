using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Destinatario
    {
        [Key]
        public int DestinatarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Ciudad { get; set; }

        [StringLength(100)]
        public string Pais { get; set; }

        // Un destinatario puede recibir muchos envíos
        public ICollection<Envio> Envios { get; set; }
    }
}