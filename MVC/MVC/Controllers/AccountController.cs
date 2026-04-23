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
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
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

        public AccountController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; // 3. Asignarlo
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


        public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, int pagina = 1)
        {
            int paginador = 10;
            var query = _context.detalle_compra.AsQueryable();

            if (desde.HasValue) query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
            if (hasta.HasValue) query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);

            var totalregistros = await query.CountAsync();

            // CORRECCIÓN: Realizamos los Joins para obtener el nombre del usuario
            var datos = await (from d in query
                               join c in _context.Compras on d.idCompra equals c.id
                               join u in _context.Usuarios on c.UsuarioId equals u.id
                               select new VentaViewModel
                               {
                                   NombreUsuario = u.Nombre, // Traemos el nombre desde la tabla Usuarios
                                   idCompra = d.idCompra,
                                   VideoJuegosId = d.VideoJuegosId,
                                   cantidad = d.cantidad,
                                   total = d.total,
                                   estadoCompra = d.estadoCompra,
                                   fechaHoraTransaccion = d.fechaHoraTransaccion,
                                   codigoTransaccion = d.codigoTransaccion
                               })
                .OrderByDescending(d => d.fechaHoraTransaccion)
                .Skip((pagina - 1) * paginador)
                .Take(paginador)
                .ToListAsync();

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
                        return RedirectToAction("Dashboard", "Account");
                    }
                }
            }

            ViewBag.Error = "Credenciales incorrectas.";
            return View(model);
        }

        [HttpGet]
        [SessionAuthorize]
        public IActionResult ObtenerPrecioPromedio()
        {
            var precioPromedio = _context.VideoJuegos
                .Include(v => v.Categoria)
                .GroupBy(v => v.Categoria.Nombre)
                .Select(g => new
                {
                    name = g.Key,
                    y = Math.Round (g.Average(v => v.Precio) , 2)
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
                if (string.IsNullOrEmpty(nombreUsuario)) return Json(new { success = false });

                var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);

                if (usuario != null)
                {
                    try
                    {
                        // 1. Registro de Cabecera de Compra
                        var nuevaCompra = new Compra
                        {
                            FechaCompra = DateTime.Now,
                            UsuarioId = usuario.id
                        };
                        _context.Compras.Add(nuevaCompra);
                        await _context.SaveChangesAsync();

                        // 2. Registro de Detalle de Compra
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

                        return Json(new { success = true });
                    }
                    catch
                    {
                        return Json(new { success = false });
                    }
                }
            }

            return Json(new { success = false });
        }
    }
}