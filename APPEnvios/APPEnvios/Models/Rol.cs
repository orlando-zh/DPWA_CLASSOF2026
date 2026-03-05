using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreRol { get; set; }

        [StringLength(150)]
        public string? Descripcion { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }
    }
}