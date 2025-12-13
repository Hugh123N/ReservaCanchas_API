using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class UploadImagenesCanchaCommandValidator : CommandValidatorBase<UploadImagenesCanchaCommand>
    {
        public UploadImagenesCanchaCommandValidator()
        {
            RuleFor(x => x.IdCancha)
                .GreaterThan(0)
                .WithMessage("ID de cancha inválido");

            RuleFor(x => x.Files)
                .NotNull()
                .WithMessage("Debe proporcionar al menos un archivo")
                .Must(files => files != null && files.Count > 0)
                .WithMessage("Debe proporcionar al menos un archivo")
                .Must(files => files.Count <= 4)
                .WithMessage("Máximo 4 imágenes por request");

            RuleForEach(x => x.Files).ChildRules(file =>
            {
                file.RuleFor(f => f.Length)
                    .LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Tamaño máximo por imagen: 5MB");

                file.RuleFor(f => f.FileName)
                    .Must(fileName => IsValidImageExtension(fileName))
                    .WithMessage("Solo se permiten imágenes: .jpg, .jpeg, .png, .webp");
            });

            // Validar que el índice principal esté en rango válido
            RuleFor(x => x.IndicePrincipal)
                .Must((command, indicePrincipal) =>
                {
                    if (!indicePrincipal.HasValue)
                        return true; // Es válido si es null

                    return indicePrincipal.Value >= 0 && indicePrincipal.Value < command.Files.Count;
                })
                .WithMessage("El índice de imagen principal está fuera de rango");
        }

        private bool IsValidImageExtension(string fileName)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(fileName).ToLower();
            return validExtensions.Contains(extension);
        }
    }
}
