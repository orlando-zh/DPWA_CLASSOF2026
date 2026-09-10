using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers // Ajusta el namespace si tu proyecto se llama distinto
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        // Inyectamos el servicio que ya configuraste en el Program.cs
        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _usuarioService.ObtenerTodos();

            // Retorna un status 200 OK junto con la lista de usuarios
            return Ok(usuarios);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioDTO dto)
        {
            // Validación básica por si llega nulo
            if (dto == null)
            {
                return BadRequest("El DTO no puede ser nulo.");
            }

            var nuevoUsuario = await _usuarioService.CrearUsuario(dto);

            // Retorna un status 200 OK junto con el usuario recién creado (y su nuevo ID)
            return Ok(nuevoUsuario);
        }
    }
}