using APPEnvios.Data;
using APPEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace APPEnvios.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context) { _context = context; }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            // Buscamos el usuario en la base de datos
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Password == password);

            if (user != null)
            {
                // Si existe, lo mandamos al Index del Home (Tu Dashboard de Admin)
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public IActionResult Logout() => RedirectToAction("Login");
    }
}