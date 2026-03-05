using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APPEnvios.Data;
using APPEnvios.Models;

namespace APPEnvios.Controllers
{
    public class EnviosController : Controller
    {
        private readonly AppDbContext _context;

        public EnviosController(AppDbContext context)
        {
            _context = context;
        }

        // Listado de envíos
        public async Task<IActionResult> Index()
        {
            var envios = await _context.Envios
                .Where(e => e.Activo == true)
                .Include(e => e.Destinatario)
                .Include(e => e.Estado)
                .Include(e => e.EstadoPago) 
                .Include(e => e.Sucursal)   
                .ToListAsync();
            return View(envios);
        }

        // Crear
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Envio envio)
        {
            if (ModelState.IsValid)
            {
                _context.Envios.Add(envio);
                await _context.SaveChangesAsync();

                // Auditoría simple
                RegistrarAuditoria(envio.EnvioId, "Crear", "Registro de nuevo envío");

                return RedirectToAction(nameof(Index));
            }
            CargarListas();
            return View(envio);
        }

        // Editar (Vista)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null) return NotFound();

            CargarListas();
            return View(envio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Envio envio)
        {
            if (id != envio.EnvioId) return NotFound();

            var envioDb = await _context.Envios.FindAsync(id);
            if (envioDb == null) return NotFound();

            try
            {
                envioDb.EstadoId = envio.EstadoId;
                envioDb.EstadoPagoId = envio.EstadoPagoId;
                envioDb.Costo = envio.Costo;
                envioDb.SucursalOrigenId = envio.SucursalOrigenId;

                await _context.SaveChangesAsync();

                RegistrarAuditoria(envioDb.EnvioId, "Editar", "Actualización de estados/costos");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                CargarListas();
                return View(envio);
            }
        }

        // Eliminar (Vista)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var envio = await _context.Envios
                .Include(e => e.Destinatario)
                .FirstOrDefaultAsync(m => m.EnvioId == id);
            if (envio == null) return NotFound();
            return View(envio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio != null)
            {
                envio.Activo = false;

                RegistrarAuditoria(id, "Eliminar", "Eliminación lógica: El registro se marcó como inactivo.");

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void CargarListas()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre");
            ViewData["DestinatarioId"] = new SelectList(_context.Destinatarios, "DestinatarioId", "Nombre");
            ViewData["SucursalOrigenId"] = new SelectList(_context.Sucursales, "SucursalId", "NombreSucursal");
            ViewData["EstadoId"] = new SelectList(_context.EstadosEnvio, "EstadoId", "NombreEstado");
            ViewData["EstadoPagoId"] = new SelectList(_context.EstadosPago, "EstadoPagoId", "NombreEstado");
        }

        private void RegistrarAuditoria(int envioId, string accion, string detalle)
        {
            var auditoria = new Auditoria_Envio
            {
                EnvioId = envioId,
                UsuarioId = 1, 
                Accion = accion,
                DetalleCambio = detalle,
                FechaModificacion = DateTime.Now
            };
            _context.Auditoria_Envios.Add(auditoria);
            _context.SaveChanges();
        }
    }
}