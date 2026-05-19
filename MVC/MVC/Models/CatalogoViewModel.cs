namespace MVC.Models
{
    public class CatalogoViewModel
    {
        public List<VideoJuego> Juegos { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }

        public int? CategoriaId { get; set; }
        public string Busqueda { get; set; }
    }
}