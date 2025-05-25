using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Usuario
{
    public class UsuarioDto
    {
        public string Nombre { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Telefono { get; set; }
        public string? Imagen { get; set; }
        public int IdRol { get; set; }
        public int IdEstadoUsuario { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
