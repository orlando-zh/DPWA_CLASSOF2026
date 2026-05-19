namespace MVC.Models
{
    public class VentasReporteViewModel
    {
        public List<VentaViewModel> Ventas { get; set; }
        public DateTime? FiltroDesde { get; set; }
        public DateTime? FiltroHasta { get; set; }
        public string FiltroCliente { get; set; }
        public string FiltroJuego { get; set; }
        public string FiltroEstado { get; set; }
    }
}