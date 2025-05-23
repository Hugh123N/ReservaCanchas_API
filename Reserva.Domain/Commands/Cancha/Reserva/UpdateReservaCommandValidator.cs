using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class UpdateReservaCommandValidator : CommandValidatorBase<UpdateReservaCommand>
    {
        private readonly IRepository<Entity.Models.Reserva> _repositoryBase;
        public UpdateReservaCommandValidator(IRepository<Entity.Models.Reserva> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdReserva, Resources.Cancha.Reserva.IdReserva)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdReserva)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Reserva.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Reserva.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateReservaCommand command, int id, ValidationContext<UpdateReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
