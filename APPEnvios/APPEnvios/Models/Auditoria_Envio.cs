using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Auditoria_Envio
    {
        [Key]
        public int AuditoriaId { get; set; }

        [Required]
        public int EnvioId { get; set; }
        [ForeignKey("EnvioId")]
        public Envio? Envio { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Accion { get; set; }

        [Required]
        [StringLength(255)]
        public string DetalleCambio { get; set; }
    }
}