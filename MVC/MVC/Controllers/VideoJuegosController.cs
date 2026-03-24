using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Data;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using System.IO;

namespace MVC.Controllers
{
    public class VideoJuegosController : Controller
    {
        public readonly AppDbContext _context;

        public VideoJuegosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var juegos = await _context.VideoJuegos.ToListAsync();
            return View(juegos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoJuego juego, IFormFile? archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(juego);

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes");
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);
                var ruta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await archivoImagen.CopyToAsync(stream);
                }

                juego.imagen = "/imagenes/" + nombreArchivo;
            }

            _context.VideoJuegos.Add(juego);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego == null) return NotFound();
            return View(juego);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VideoJuego juego, IFormFile? archivoImagen)
        {
            if (id != juego.id)
                return NotFound();

            var juegoBD = await _context.VideoJuegos.FindAsync(id);

            if (juegoBD == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                juegoBD.Titulo = juego.Titulo;
                juegoBD.Precio = juego.Precio;
                juegoBD.Categoria = juego.Categoria;
                juegoBD.Descripcion = juego.Descripcion;

                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    // SOLUCIÓN: Crear carpeta si no existe
                    var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes");
                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    if (!string.IsNullOrEmpty(juegoBD.imagen))
                    {
                        var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", juegoBD.imagen.TrimStart('/'));

                        if (System.IO.File.Exists(rutaAnterior))
                            System.IO.File.Delete(rutaAnterior);
                    }

                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);
                    var rutaNueva = Path.Combine(rutaCarpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaNueva, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }
                    juegoBD.imagen = "/imagenes/" + nombreArchivo;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            juego.imagen = juegoBD.imagen;

            return View(juego);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos
                .FirstOrDefaultAsync(m => m.id == id);

            if (juego == null) return NotFound();

            return View(juego);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego != null)
            {
                if (!string.IsNullOrEmpty(juego.imagen))
                {
                    var rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", juego.imagen.TrimStart('/'));
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }
                }

                _context.VideoJuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}