using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class DetalleCompra
    {
        public int Id { get; set; }

        public int VideoJuegosId { get; set; }

        [ForeignKey("VideoJuegosId")]
        public virtual VideoJuego VideoJuego { get; set; }

        public int cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal total { get; set; }

        public string estadoCompra { get; set; }

        public DateTime fechaHoraTransaccion { get; set; }

        public string codigoTransaccion { get; set; }

        public int idCompra { get; set; }

        [ForeignKey("idCompra")]
        public virtual Compra Compra { get; set; }

    }
}