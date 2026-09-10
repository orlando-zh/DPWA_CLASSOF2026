using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Usuario>> ObtenerTodos()
        {
            return await _repository.ObtenerTodos();
        }

        public async Task<Usuario> CrearUsuario(UsuarioDTO dto)
        {
            Usuario usuario = new Usuario()
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo
            };

            return await _repository.Crear(usuario);
        }
    }
}
