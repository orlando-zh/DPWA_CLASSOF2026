using System;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class VentaViewModel
    {
        public string NombreUsuario { get; set; }

        public int idCompra { get; set; }

        public DateTime FechaCompra { get; set; } = DateTime.Now;

        public int UsuarioId { get; set; }

        public int VideoJuegosId { get; set; }

        public int cantidad { get; set; }

        [Required]
        public decimal total { get; set; }

        public string estadoCompra { get; set; }

        public DateTime fechaHoraTransaccion { get; set; } = DateTime.Now;

        public string codigoTransaccion { get; set; }
    }
}