using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Data;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

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
    }
}
