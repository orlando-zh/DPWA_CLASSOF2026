using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace APPEnvios.Models
{
    public class Sucursal
    {
        [Key]
        public int SucursalId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreSucursal { get; set; }

        [Required]
        [StringLength(100)]
        public string Departamento { get; set; }

        public ICollection<Envio>? Envios { get; set; }
    }
}