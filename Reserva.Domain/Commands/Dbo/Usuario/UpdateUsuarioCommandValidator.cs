using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class UpdateUsuarioCommandValidator : CommandValidatorBase<UpdateUsuarioCommand>
    {
        private readonly IRepository<Entity.AspNetUsers> _repositoryBase;
        public UpdateUsuarioCommandValidator(IRepository<Entity.AspNetUsers> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            // Validación para el campo Imagen (opcional, debe ser base64 válido si está presente)
            RuleFor(x => x.UpdateDto.Imagen)
                .Must(BeValidBase64OrNull)
                .WithMessage("La imagen debe ser una cadena base64 válida o estar vacía.");

            /*RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.Id, Resources.Dbo.Usuario.IdUsuario)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.Id)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Usuario.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Usuario.FechaIngreso);
            });*/
        }

        private bool BeValidBase64OrNull(string? imagen)
        {
            // Si es null o vacío, es válido (campo opcional)
            if (string.IsNullOrWhiteSpace(imagen))
                return true;

            // Si contiene el prefijo data:image, extraer solo la parte base64
            if (imagen.Contains("data:image"))
            {
                var base64Index = imagen.IndexOf("base64,");
                if (base64Index != -1)
                {
                    imagen = imagen.Substring(base64Index + 7);
                }
            }

            // Validar que sea base64 válido
            try
            {
                Span<byte> buffer = new Span<byte>(new byte[imagen.Length]);
                return Convert.TryFromBase64String(imagen, buffer, out _);
            }
            catch
            {
                return false;
            }
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateUsuarioCommand command, Guid id, ValidationContext<UpdateUsuarioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.Id == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
