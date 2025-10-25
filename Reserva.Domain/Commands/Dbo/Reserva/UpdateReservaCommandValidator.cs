using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class UpdateReservaCommandValidator : CommandValidatorBase<UpdateReservaCommand>
    {
        private readonly IRepository<Entity.Reserva> _repositoryBase;
        public UpdateReservaCommandValidator(IRepository<Entity.Reserva> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdReserva, Resources.Dbo.Reserva.IdReserva)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdReserva)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Reserva.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Reserva.FechaIngreso);
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
