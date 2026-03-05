using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Envio
    {
        [Key]
        public int EnvioId { get; set; }

        [Required]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        [Required]
        public int DestinatarioId { get; set; }
        [ForeignKey("DestinatarioId")]
        public Destinatario? Destinatario { get; set; }

        [Required]
        public int EstadoId { get; set; } = 1;
        [ForeignKey("EstadoId")]
        public EstadoEnvio? Estado { get; set; }

        public int? SucursalOrigenId { get; set; }
        [ForeignKey("SucursalOrigenId")]
        public Sucursal? Sucursal { get; set; }

        public int EstadoPagoId { get; set; } = 1; 
        [ForeignKey("EstadoPagoId")]
        public EstadoPago? EstadoPago { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        public DateTime? FechaEntrega { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; }

        public ICollection<Paquete>? Paquetes { get; set; }
        public ICollection<Auditoria_Envio>? Auditorias { get; set; }

        public bool Activo { get; set; } = true;
    }
}