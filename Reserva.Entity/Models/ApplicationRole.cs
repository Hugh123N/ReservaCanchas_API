using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Entity.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {

        [Required]
        [MaxLength(64)]
        public string UserNameCreate { get; set; } = null!;

        [Required]
        public DateTimeOffset CreateDate { get; set; }

        [MaxLength(64)]
        public string? UserNameUpdate { get; set; }

        public DateTimeOffset? UpdateDate { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}
