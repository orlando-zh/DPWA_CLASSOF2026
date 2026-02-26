using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;



namespace APPEnvios.Models
{
    public class Paquete
    {
        [Key]
        public int PaqueteId { get; set; }

        [Required]
        public int EnvioId { get; set; }
        [ForeignKey("EnvioId")]
        public Envio Envio { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Peso { get; set; }
    }
}