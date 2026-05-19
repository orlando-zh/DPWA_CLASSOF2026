using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Data;
using MVC.Models;
using System.Security.Cryptography; 
using System.Text; 
using System.Linq;
using appWeb2.Filtros;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MVC.Migrations;
using Rotativa.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;

namespace MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext context, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _context = context;
            _configuration = configuration; // 3. Asignarlo
            _logger = logger;
        }

        [SessionAuthorize]

        public IActionResult Dashboard()
        {
            var categorias = _context.Categorias
                .Select(c => new { c.idCategoria, c.Nombre })
                .OrderBy(c => c.Nombre)
                .ToList();
            ViewBag.Categorias = categorias;
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public IActionResult ObtenerDatos(int? idCategoria)
        {
            var query = from v in _context.VideoJuegos
                        join c in _context.Categorias on v.idCategoria equals c.idCategoria
                        select new
                        {
                            Categoria = c.Nombre,
                            Juego = v.Titulo,
                            idCategoria = c.idCategoria
                        };

            if (idCategoria.HasValue)
                query = query.Where(x => x.idCategoria == idCategoria.Value);

            var data = query
                .GroupBy(x => x.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    Total = g.Count()
                })
                .ToList();

            return Json(data);
        }


        public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, string cliente, string juego, string estado, int pagina = 1)
        {
            int paginador = 10;

            // 1. Cargamos las listas para los Selects (esto es lo nuevo)
            ViewBag.ListaClientes = await _context.Usuarios
                .OrderBy(u => u.Nombre)
                .Select(u => u.Nombre)
                .ToListAsync();

            ViewBag.ListaJuegos = await _context.VideoJuegos
                .OrderBy(v => v.Titulo)
                .Select(v => v.Titulo)
                .ToListAsync();

            // 2. Tu consulta con Joins[cite: 15]
            var query = (from d in _context.detalle_compra
                         join c in _context.Compras on d.idCompra equals c.id
                         join u in _context.Usuarios on c.UsuarioId equals u.id
                         join v in _context.VideoJuegos on d.VideoJuegosId equals v.id
                         select new VentaViewModel
                         {
                             NombreUsuario = u.Nombre,
                             idCompra = d.idCompra,
                             VideoJuegosId = d.VideoJuegosId,
                             Titulo = v.Titulo,
                             cantidad = d.cantidad,
                             total = d.total,
                             estadoCompra = d.estadoCompra,
                             fechaHoraTransaccion = d.fechaHoraTransaccion,
                             codigoTransaccion = d.codigoTransaccion
                         }).AsQueryable();

            // 3. Aplicación de filtros
            if (desde.HasValue) query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
            if (hasta.HasValue) query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);
            if (!string.IsNullOrEmpty(cliente)) query = query.Where(d => d.NombreUsuario == cliente);
            if (!string.IsNullOrEmpty(juego)) query = query.Where(d => d.Titulo == juego);
            if (!string.IsNullOrEmpty(estado)) query = query.Where(d => d.estadoCompra == estado);

            var totalregistros = await query.CountAsync();

            var datos = await query
                .OrderByDescending(d => d.fechaHoraTransaccion)
                .Skip((pagina - 1) * paginador)
                .Take(paginador)
                .ToListAsync();

            // 4. Pasar valores seleccionados de vuelta a la vista
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.ClienteSelected = cliente;
            ViewBag.JuegoSelected = juego;
            ViewBag.EstadoSelected = estado;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalregistros / paginador);
            ViewBag.PaginaActual = pagina;

            return View(datos);
        }



        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Usuarios.FirstOrDefault(u => u.Email == model.Email);

            if (user != null)
            {
                string saltedPassword = user.Salt + model.Password;

                using (SHA256 sha256 = SHA256.Create())
                {
                    // CAMBIO: Ahora usamos Encoding.Unicode (UTF-16)
                    byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                    byte[] hashGenerado = sha256.ComputeHash(inputBytes);

                    // Comparación de arreglos de bytes
                    if (hashGenerado.SequenceEqual(user.Password))
                    {
                        HttpContext.Session.SetString("Usuario", user.Nombre);
                        HttpContext.Session.SetInt32("idrol", (int)user.idRol);
                        if (user.idRol == 1)
                        {
                            return RedirectToAction("Dashboard", "Account");
                        }
                        else if (user.idRol == 2)
                        {
                            return RedirectToAction("Catalogo", "Home");
                        }
                        {
                            return RedirectToAction("Catalogo", "Home");
                        }

                    }
                }
            }

            ViewBag.Error = "Credenciales incorrectas.";
            return View(model);
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string nombre, string email, string password)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Email == email);
            if (existe)
            {
                // Como ahora es su propia vista, volvemos a usar ViewBag
                ViewBag.Error = "El correo ya está registrado en el sistema.";
                return View();
            }

            string query = @"
        DECLARE @salt NVARCHAR(50) = CAST(NEWID() AS NVARCHAR(50));
        
        INSERT INTO Usuarios (Nombre, Email, Password, Salt, FechaRegistro, idRol)
        VALUES (
            {0}, 
            {1}, 
            HASHBYTES('SHA2_256', @salt + CAST({2} AS NVARCHAR(MAX))), 
            @salt, 
            GETDATE(), 
            2
        );";

            await _context.Database.ExecuteSqlRawAsync(query, nombre, email, password);

            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        [SessionAuthorize]
        public IActionResult ObtenerPrecioPromedio(int? idCategoria)
        {
            var query = _context.VideoJuegos.Include(v => v.Categoria).AsQueryable();

            if (idCategoria.HasValue)
            {
                query = query.Where(v => v.idCategoria == idCategoria.Value);
            }

            var precioPromedio = query
                .GroupBy(v => v.Categoria.Nombre)
                .Select(g => new
                {
                    name = g.Key,
                    y = Math.Round(g.Average(v => v.Precio), 2)
                })
                .ToList();

            return Json(precioPromedio);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }



        [HttpPost]
        public async Task<IActionResult> CrearOrden(decimal monto, string nombreJuego)
        {
            var clientId = _configuration["PayPal:ClientId"];
            var secret = _configuration["PayPal:Secret"];
            var baseUrl = _configuration["PayPal:BaseUrl"];

            using var client = new HttpClient();
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var orderRequest = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
            new {
                amount = new { currency_code = "USD", value = monto.ToString("F2") },
                description = nombreJuego
            }
        }
            };

            var response = await client.PostAsJsonAsync($"{baseUrl}/v2/checkout/orders", orderRequest);
            var jsonResponse = await response.Content.ReadFromJsonAsync<PayPalOrderResponse>();

            return Json(new { id = jsonResponse.id });
        }

        [HttpPost]
        public async Task<IActionResult> CapturarOrden(string orderId, int videoJuegoId, int cantidad, decimal total)
        {
            var clientId = _configuration["PayPal:ClientId"];
            var secret = _configuration["PayPal:Secret"];
            var baseUrl = _configuration["PayPal:BaseUrl"];

            using var client = new HttpClient();
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            // Enviamos el formato correcto (application/json) para evitar el error 400 de PayPal
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/v2/checkout/orders/{orderId}/capture", content);

            if (response.IsSuccessStatusCode)
            {
                var nombreUsuario = HttpContext.Session.GetString("Usuario");
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);

                if (usuario != null)
                {
                    try
                    {
                        var nuevaCompra = new Compra { FechaCompra = DateTime.Now, UsuarioId = usuario.id };
                        _context.Compras.Add(nuevaCompra);
                        await _context.SaveChangesAsync(); 

                var detalle = new DetalleCompra
                {
                    idCompra = nuevaCompra.id,
                    VideoJuegosId = videoJuegoId,
                    cantidad = cantidad,
                    total = total,
                    estadoCompra = "Completado",
                    fechaHoraTransaccion = DateTime.Now,
                    codigoTransaccion = orderId
                };
                        _context.detalle_compra.Add(detalle);
                        await _context.SaveChangesAsync();

                        // CAMBIO CLAVE: Devolvemos el ID de la compra para el resumen
                        return Json(new { success = true, compraId = nuevaCompra.id });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }
            return Json(new { success = false });
        }


        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> ExportarPDF(DateTime? desde, DateTime? hasta, string cliente, string juego, string estado, CancellationToken cancellationToken)
        {
            // 1. Filtros a la vista
            ViewBag.FiltroDesde = desde;
            ViewBag.FiltroHasta = hasta;
            ViewBag.FiltroCliente = cliente;
            ViewBag.FiltroJuego = juego;
            ViewBag.FiltroEstado = estado;

            // 2. Consulta con AsNoTracking para mejor rendimiento
            var query = _context.detalle_compra
                .Include(d => d.Compra)
                    .ThenInclude(c => c.Usuario)
                .Include(d => d.VideoJuego)
                .AsNoTracking()
                .AsQueryable();

            // 3. Filtros
            if (desde.HasValue) query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
            if (hasta.HasValue) query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);
            if (!string.IsNullOrWhiteSpace(cliente)) query = query.Where(d => d.Compra.Usuario.Nombre == cliente);
            if (!string.IsNullOrWhiteSpace(juego)) query = query.Where(d => d.VideoJuego.Titulo == juego);
            if (!string.IsNullOrEmpty(estado)) query = query.Where(d => d.estadoCompra == estado);

            // 4. Ejecución
            var datos = await query
                .OrderByDescending(d => d.fechaHoraTransaccion)
                .Select(d => new VentaViewModel
                {
                    NombreUsuario = d.Compra.Usuario.Nombre,
                    idCompra = d.idCompra,
                    VideoJuegosId = d.VideoJuegosId,
                    Titulo = d.VideoJuego.Titulo,
                    cantidad = d.cantidad,
                    total = d.total,
                    estadoCompra = d.estadoCompra,
                    fechaHoraTransaccion = d.fechaHoraTransaccion,
                    codigoTransaccion = d.codigoTransaccion
                })
                .ToListAsync(cancellationToken);

            try
            {
                // Empaquetamos absolutamente todo en un solo objeto fuertemente tipado
                var reporte = new VentasReporteViewModel
                {
                    Ventas = datos,
                    FiltroDesde = desde,
                    FiltroHasta = hasta,
                    FiltroCliente = cliente,
                    FiltroJuego = juego,
                    FiltroEstado = estado
                };

                // Rotativa recibe el modelo completo, imposible que pierda los filtros
                return new ViewAsPdf("PdfVentas", reporte)
                {
                    FileName = "Reporte_Ventas_GameStore.pdf"
                };
            }
            catch (OperationCanceledException)
            {
                return BadRequest("La operación fue cancelada.");
            }
            catch (Exception ex)
            {
                return Content("Error al generar PDF: " + ex.Message);
            }
        }


        [SessionAuthorize]
        public async Task<IActionResult> MisCompras(DateTime? desde, DateTime? hasta, string nombreJuego)
        {
            // 1. Obtener el nombre del usuario de la sesión para buscar su ID
            var nombreUsuario = HttpContext.Session.GetString("Usuario");
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);

            if (usuario == null) return RedirectToAction("Login");

            // 2. Consulta base: Incluimos la cabecera (Compra) y el juego
            var query = _context.detalle_compra
                .Include(d => d.Compra)
                .Include(d => d.VideoJuego)
                .Where(d => d.Compra.UsuarioId == usuario.id)
                .AsQueryable();

            // 3. Aplicar filtros dinámicos solicitados
            if (desde.HasValue)
                query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);

            if (!string.IsNullOrEmpty(nombreJuego))
                query = query.Where(d => d.VideoJuego.Titulo.Contains(nombreJuego));

            // 4. Construir el modelo para la vista
            var model = new MisComprasViewModel
            {
                Compras = await query.OrderByDescending(d => d.fechaHoraTransaccion).ToListAsync(),
                Desde = desde,
                Hasta = hasta,
                NombreJuego = nombreJuego
            };

            return View(model);
        }


        [SessionAuthorize]
        public async Task<IActionResult> ConfirmacionCompra(int id)
        {
            if (id <= 0) return RedirectToAction("Catalogo", "Home");

            // AsNoTracking() hace que la consulta sea mucho más ligera y evita el crash -1
            var compra = await _context.detalle_compra
                .Include(d => d.VideoJuego)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.idCompra == id);

            if (compra == null) return RedirectToAction("MisCompras");

            return View(compra);
        }




        [SessionAuthorize]
        public async Task<IActionResult> Perfil()
        {
            var nombreUsuario = HttpContext.Session.GetString("Usuario");
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);

            if (usuario == null) return RedirectToAction("Login");

            return View(usuario);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarNombre(string nuevoNombre)
        {
            var nombreActual = HttpContext.Session.GetString("Usuario");
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombreActual);

            if (usuario != null && !string.IsNullOrEmpty(nuevoNombre))
            {
                usuario.Nombre = nuevoNombre;
                await _context.SaveChangesAsync();

                // Actualizamos la sesión para que el nombre cambie en el Layout inmediatamente
                HttpContext.Session.SetString("Usuario", nuevoNombre);
                TempData["Success"] = "Nombre actualizado correctamente.";
            }
            return RedirectToAction("Perfil");
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(string passwordActual, string nuevaPassword)
        {
            var nombreUsuario = HttpContext.Session.GetString("Usuario");
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);

            if (usuario != null)
            {
                // 1. Verificar contraseña actual usando el Salt guardado
                string saltedActual = usuario.Salt + passwordActual;
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashActual = sha256.ComputeHash(Encoding.Unicode.GetBytes(saltedActual));
                    if (!hashActual.SequenceEqual(usuario.Password))
                    {
                        TempData["Error"] = "La contraseña actual es incorrecta.";
                        return RedirectToAction("Perfil");
                    }

                    // 2. Hashear la nueva contraseña con el mismo Salt
                    string saltedNueva = usuario.Salt + nuevaPassword;
                    usuario.Password = sha256.ComputeHash(Encoding.Unicode.GetBytes(saltedNueva));

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Contraseña actualizada con éxito.";
                }
            }
            return RedirectToAction("Perfil");
        }


        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> ObtenerVentasPorMes(DateTime? desde, DateTime? hasta, int? idCategoria)
        {
            // Incluimos VideoJuego para poder filtrar por su categoría
            var query = _context.detalle_compra
                .Include(d => d.VideoJuego)
                .AsNoTracking()
                .AsQueryable();

            if (desde.HasValue) query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
            if (hasta.HasValue) query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);
            if (idCategoria.HasValue) query = query.Where(d => d.VideoJuego.idCategoria == idCategoria.Value);

            var data = await query
                .GroupBy(d => new { d.fechaHoraTransaccion.Year, d.fechaHoraTransaccion.Month })
                .Select(g => new
                {
                    ano = g.Key.Year,
                    mes = g.Key.Month,
                    total = g.Sum(x => x.total)
                })
                .OrderBy(x => x.ano).ThenBy(x => x.mes)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> ObtenerTopJuegos(DateTime? desde, DateTime? hasta, int? idCategoria)
        {
            var query = _context.detalle_compra
                .Include(d => d.VideoJuego)
                .AsNoTracking()
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);

            if (idCategoria.HasValue)
                query = query.Where(d => d.VideoJuego.idCategoria == idCategoria.Value);

            var data = await query
                .GroupBy(d => d.VideoJuego.Titulo)
                .Select(g => new
                {
                    juego = g.Key,
                    total = g.Sum(x => x.cantidad)
                })
                .OrderByDescending(x => x.total)
                .Take(5)
                .ToListAsync();

            return Json(data);
        }

    }
}