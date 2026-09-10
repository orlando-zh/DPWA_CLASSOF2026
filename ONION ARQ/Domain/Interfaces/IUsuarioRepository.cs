using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ObtenerTodos();

        Task<Usuario> Crear(Usuario usuario);
    }
}
