using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class UpdateZonaCommandValidator : CommandValidatorBase<UpdateZonaCommand>
    {
        private readonly IRepository<Entity.Models.Zona> _repositoryBase;
        public UpdateZonaCommandValidator(IRepository<Entity.Models.Zona> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdZona, Resources.Cancha.Zona.IdZona)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdZona)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Zona.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Zona.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateZonaCommand command, int id, ValidationContext<UpdateZonaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdZona == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
