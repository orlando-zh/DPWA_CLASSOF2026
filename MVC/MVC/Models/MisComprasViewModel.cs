namespace MVC.Models
{
    public class MisComprasViewModel
    {
        // Corregido: Usamos DetalleCompra que es el tipo real en tu proyecto
        public List<DetalleCompra> Compras { get; set; } = new List<DetalleCompra>();

        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string? NombreJuego { get; set; }
    }
}