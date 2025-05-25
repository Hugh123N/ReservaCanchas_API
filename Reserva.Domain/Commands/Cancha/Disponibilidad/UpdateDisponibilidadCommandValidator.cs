using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class UpdateDisponibilidadCommandValidator : CommandValidatorBase<UpdateDisponibilidadCommand>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _repositoryBase;
        public UpdateDisponibilidadCommandValidator(IRepository<Entity.Models.Disponibilidad> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDisponibilidad, Resources.Cancha.Disponibilidad.IdDisponibilidad)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDisponibilidad)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Disponibilidad.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Disponibilidad.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDisponibilidadCommand command, int id, ValidationContext<UpdateDisponibilidadCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDisponibilidad == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
