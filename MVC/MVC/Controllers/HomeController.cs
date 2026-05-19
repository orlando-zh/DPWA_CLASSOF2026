using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 PORTADA (Landing Page)
        public async Task<IActionResult> Index()
        {
            // Traemos los últimos 4 juegos para mostrarlos como destacados en la portada
            var destacados = await _context.VideoJuegos
                .Include(j => j.Categoria)
                .OrderByDescending(j => j.id)
                .Take(4)
                .ToListAsync();
            return View(destacados);
        }

        // 🔹 CATÁLOGO COMPLETO
        public async Task<IActionResult> Catalogo(int? idCategoria, string? buscar, int pagina = 1)
        {
            int registrosPorPagina = 10; // Requerimiento estricto

            var consulta = _context.VideoJuegos
                .Include(j => j.Categoria)
                .Include(j => j.Promocion)
                .AsQueryable();

            // 1. Filtrado por Categoría
            if (idCategoria.HasValue)
            {
                consulta = consulta.Where(j => j.idCategoria == idCategoria);
            }

            // 2. Filtrado por Nombre
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(j => j.Titulo.Contains(buscar));
            }

            // 3. Cálculo de Paginación
            var totalRegistros = await consulta.CountAsync();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            // 4. Obtención de datos con Skip y Take[cite: 1, 5]
            var juegos = await consulta
                .OrderBy(j => j.Titulo) // Ordenar es obligatorio para Skip/Take
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            // 5. Preparar el modelo para la vista
            var model = new CatalogoViewModel
            {
                Juegos = juegos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                CategoriaId = idCategoria,
                Busqueda = buscar
            };

            // Mantener la lista de categorías para el menú lateral/filtro
            ViewBag.Categorias = await _context.Categorias.ToListAsync();

            return View(model);
        }

        // 🔹 PROMOCIONES VIGENTES
        public async Task<IActionResult> Promociones()
        {
            // Es vital el .Include para que el objeto Promocion no sea null en la vista
            var juegosConOferta = await _context.VideoJuegos
                .Include(v => v.Promocion)
                .Where(v => v.PromocionId != null) // Filtramos solo los que tienen oferta vinculada
                .ToListAsync();

            return View(juegosConOferta);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int id)
        {
            var juego = await _context.VideoJuegos
                .Include(j => j.Categoria)
                .Include(j => j.Promocion)
                .FirstOrDefaultAsync(m => m.id == id);

            if (juego == null) return NotFound();

            return View(juego);
        }
    }

}