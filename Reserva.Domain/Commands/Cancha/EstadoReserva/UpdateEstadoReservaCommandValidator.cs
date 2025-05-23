using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class UpdateEstadoReservaCommandValidator : CommandValidatorBase<UpdateEstadoReservaCommand>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _repositoryBase;
        public UpdateEstadoReservaCommandValidator(IRepository<Entity.Models.EstadoReserva> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdEstadoReserva, Resources.Cancha.EstadoReserva.IdEstadoReserva)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdEstadoReserva)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.EstadoReserva.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.EstadoReserva.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateEstadoReservaCommand command, int id, ValidationContext<UpdateEstadoReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
