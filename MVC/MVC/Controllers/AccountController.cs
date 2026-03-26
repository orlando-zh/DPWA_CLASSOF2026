using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Data;
using MVC.Models;
using System.Security.Cryptography; 
using System.Text; 
using System.Linq;

namespace MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
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
            // 1. Buscamos al usuario por su Email
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Email);

            // 2. Si el usuario existe, validamos el hash
            if (user != null)
            {
                // Combinamos la sal de la DB con el password que viene del formulario
                string saltedPassword = user.Salt + model.Password;

                using (SHA256 sha256 = SHA256.Create())
                {
                    // Convertimos el string a bytes y generamos el hash
                    byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    if (hashBytes.SequenceEqual(user.Password))
                    {
                        HttpContext.Session.SetString("Usuario", user.Nombre);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        public void CambiarPassword(int userId, string nuevaPassword)
        {
            var user = _context.Usuarios.Find(userId);

            if (user != null)
            {
                string nuevoSalt = Guid.NewGuid().ToString();
                string saltedPassword = nuevoSalt + nuevaPassword;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] hash = sha256.ComputeHash(bytes);

                    user.Salt = nuevoSalt;
                    user.Password = hash;
                }

                _context.SaveChanges();
            }
        }

        public IActionResult ResetTest()
        {
            CambiarPassword(1, "123456"); // 👈 ID del usuario
            return Content("Contraseña actualizada");
        }
    }
}