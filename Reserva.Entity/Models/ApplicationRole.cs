using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Entity.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public Guid ApplicationId { get; set; }

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
