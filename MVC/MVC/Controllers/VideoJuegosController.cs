using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Data;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using System.IO;
using appWeb2.Filtros;
using System;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    [SessionAuthorize]
    public class VideoJuegosController : Controller
    {
        private readonly AppDbContext _context;

        public VideoJuegosController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 INVENTARIO
        public async Task<IActionResult> Inventario()
        {
            var juegos = await _context.VideoJuegos
                .Include(j => j.Categoria)
                .Include(j => j.Promocion)
                .ToListAsync();
            return View(juegos);
        }

        // 🔹 CREATE (GET)
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, "idCategoria", "Nombre");
            ViewBag.Promociones = new SelectList(_context.Promociones, "Id", "Nombre");
            return View();
        }

        // 🔹 CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoJuego juego, IFormFile? archivoImagen)
        {
            ModelState.Remove("Categoria");
            ModelState.Remove("Promocion");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.Categorias, "idCategoria", "Nombre", juego.idCategoria);
                ViewBag.Promociones = new SelectList(_context.Promociones, "Id", "Nombre", juego.PromocionId);
                return View(juego);
            }

            try
            {
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                    if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);
                    var rutaFinal = Path.Combine(rutaCarpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaFinal, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }
                    juego.imagen = "/imagenes/" + nombreArchivo;
                }

                _context.VideoJuegos.Add(juego);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Inventario));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el juego: " + ex.Message);
                ViewBag.Categorias = new SelectList(_context.Categorias, "idCategoria", "Nombre", juego.idCategoria);
                ViewBag.Promociones = new SelectList(_context.Promociones, "Id", "Nombre", juego.PromocionId);
                return View(juego);
            }
        }

        // 🔹 EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego == null) return NotFound();

            ViewBag.Categorias = new SelectList(_context.Categorias, "idCategoria", "Nombre", juego.idCategoria);
            ViewBag.Promociones = new SelectList(_context.Promociones, "Id", "Nombre", juego.PromocionId);

            return View(juego);
        }

        // 🔹 EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VideoJuego juego, IFormFile? archivoImagen)
        {
            ModelState.Remove("Categoria");
            ModelState.Remove("Promocion");

            if (id != juego.id) return NotFound();

            var juegoBD = await _context.VideoJuegos.FindAsync(id);
            if (juegoBD == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    juegoBD.Titulo = juego.Titulo;
                    juegoBD.Precio = juego.Precio;
                    juegoBD.Descripcion = juego.Descripcion;
                    juegoBD.EdadMinima = juego.EdadMinima;
                    juegoBD.idCategoria = juego.idCategoria;
                    juegoBD.PromocionId = juego.PromocionId;

                    if (archivoImagen != null && archivoImagen.Length > 0)
                    {
                        var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                        if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                        if (!string.IsNullOrEmpty(juegoBD.imagen))
                        {
                            var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", juegoBD.imagen.TrimStart('/'));
                            if (System.IO.File.Exists(rutaAnterior)) System.IO.File.Delete(rutaAnterior);
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
                    return RedirectToAction(nameof(Inventario));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            ViewBag.Categorias = new SelectList(_context.Categorias, "idCategoria", "Nombre", juego.idCategoria);
            ViewBag.Promociones = new SelectList(_context.Promociones, "Id", "Nombre", juego.PromocionId);
            juego.imagen = juegoBD.imagen;
            return View(juego);
        }

        // 🔹 DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos
                .Include(j => j.Categoria)
                .Include(j => j.Promocion)
                .FirstOrDefaultAsync(m => m.id == id);

            if (juego == null) return NotFound();

            return View(juego);
        }

        // 🔹 DELETE (POST)
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
                    if (System.IO.File.Exists(rutaImagen)) System.IO.File.Delete(rutaImagen);
                }

                _context.VideoJuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Inventario));
        }
    }
}