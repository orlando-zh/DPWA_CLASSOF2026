using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Data;
using MVC.Models;
using System.Security.Cryptography; 
using System.Text; 
using System.Linq;
using appWeb2.Filtros;

namespace MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        [SessionAuthorize]

        public IActionResult Dashboard()
        {
            var data = (from v in _context.VideoJuegos
                        join c in _context.Categorias
                        on v.CategoriaId equals c.Id 
                        group v by c.Nombre into g  
                        select new
                        {
                            Categoria = g.Key,
                            Total = g.Count()
                        }).ToList();
            ViewBag.Categorias = data.Select(x => x.Categoria).ToList();
            ViewBag.Totales = data.Select(x => x.Total).ToList();

            return View();
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}