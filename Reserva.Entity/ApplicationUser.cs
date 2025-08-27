using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Entity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }
        public string? Imagen { get; set; }

        public int IdEstadoUsuario { get; set; }

        [Required]
        [MaxLength(64)]
        public string UserNameCreate { get; set; } = null!;

        [Required]
        public DateTimeOffset CreateDate { get; set; }

        [Required]
        [MaxLength(64)]
        public string UserNameUpdate { get; set; } = null!;

        [Required]
        public DateTimeOffset UpdateDate { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}
