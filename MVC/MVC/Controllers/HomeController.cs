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
        public async Task<IActionResult> Catalogo(int? idCategoria, string? buscar) 
        {
            var consulta = _context.VideoJuegos
                .Include(j => j.Categoria)
                .Include(j => j.Promocion)
                .AsQueryable();

            // Filtrado por Categoría
            if (idCategoria.HasValue) 
            {
                consulta = consulta.Where(j => j.idCategoria == idCategoria); 
                ViewBag.CategoriaSeleccionada = (await _context.Categorias.FindAsync(idCategoria))?.Nombre; 
            }


            // Filtrado por Nombre
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(j => j.Titulo.Contains(buscar));
                ViewBag.Busqueda = buscar;
            }

            var juegos = await consulta.ToListAsync();
            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            
            return View(juegos);
        }

        // 🔹 PROMOCIONES VIGENTES
        public async Task<IActionResult> Promociones()
        {
            var fechaActual = DateTime.Now;
            var juegos = await _context.VideoJuegos
                .Include(j => j.Categoria)
                .Include(j => j.Promocion)
                .Where(j => j.PromocionId != null 
                         && j.Promocion.FechaInicio <= fechaActual 
                         && j.Promocion.FechaFin >= fechaActual)
                .ToListAsync();

            return View(juegos);
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