using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Usuario
{
    public class UsuarioDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? UserName { get; set; } = null!;
        public string? NormalizedUserName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? NormalizedEmail { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; } = null!;
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; } = null!;
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
