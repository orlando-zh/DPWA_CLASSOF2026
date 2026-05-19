using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Data;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using System.IO;
using appWeb2.Filtros;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace MVC.Controllers
{
    [SessionAuthorize]
    public class VideoJuegosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VideoJuegosController> _logger;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedImageExtensions =
        [
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        ];

        public VideoJuegosController(AppDbContext context, ILogger<VideoJuegosController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        private string GetImagesFolderPath()
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            return Path.Combine(webRoot, "imagenes");
        }

        private static string? GetSafeFileNameFromRelativePath(string? relativePath)
            => string.IsNullOrWhiteSpace(relativePath) ? null : Path.GetFileName(relativePath);

        private async Task<string?> SaveImageAsync(IFormFile? archivoImagen)
        {
            if (archivoImagen == null || archivoImagen.Length <= 0)
                return null;

            var ext = Path.GetExtension(archivoImagen.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedImageExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Formato de imagen no permitido. Use JPG, PNG, GIF o WEBP.");

            var rutaCarpeta = GetImagesFolderPath();
            Directory.CreateDirectory(rutaCarpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{ext}";
            var rutaNueva = Path.Combine(rutaCarpeta, nombreArchivo);

            await using (var stream = new FileStream(rutaNueva, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await archivoImagen.CopyToAsync(stream);
            }

            return "/imagenes/" + nombreArchivo;
        }

        private void TryDeleteImage(string? relativeImagePath)
        {
            var fileName = GetSafeFileNameFromRelativePath(relativeImagePath);
            if (string.IsNullOrWhiteSpace(fileName)) return;

            var rutaImagen = Path.Combine(GetImagesFolderPath(), fileName);
            if (!System.IO.File.Exists(rutaImagen)) return;

            try
            {
                System.IO.File.Delete(rutaImagen);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo borrar la imagen {RutaImagen}", rutaImagen);
            }
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
                var nuevaRutaImagen = await SaveImageAsync(archivoImagen);
                if (!string.IsNullOrWhiteSpace(nuevaRutaImagen))
                    juego.imagen = nuevaRutaImagen;

                _context.VideoJuegos.Add(juego);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Inventario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo crear el juego");
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
                      // Guardar primero la nueva imagen y luego borrar la anterior (evita quedarse sin imagen si falla el guardado)
                        var nuevaRutaImagen = await SaveImageAsync(archivoImagen);
                        if (!string.IsNullOrWhiteSpace(nuevaRutaImagen))
                        {
                            var anterior = juegoBD.imagen;
                            juegoBD.imagen = nuevaRutaImagen;
                            TryDeleteImage(anterior);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Inventario));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al editar videojuego {VideoJuegoId}", id);
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
                TryDeleteImage(juego.imagen);
                        
                _context.VideoJuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Inventario));
        }
    }
}