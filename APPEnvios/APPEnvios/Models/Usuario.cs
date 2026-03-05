using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }
        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        public ICollection<Auditoria_Envio>? Auditorias { get; set; }
    }
}