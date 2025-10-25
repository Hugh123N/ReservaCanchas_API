using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoPago
{
    public class UpdateEstadoPagoCommandValidator : CommandValidatorBase<UpdateEstadoPagoCommand>
    {
        private readonly IRepository<Entity.EstadoPago> _repositoryBase;
        public UpdateEstadoPagoCommandValidator(IRepository<Entity.EstadoPago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdEstadoPago, Resources.Dbo.EstadoPago.IdEstadoPago)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdEstadoPago)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.EstadoPago.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.EstadoPago.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateEstadoPagoCommand command, int id, ValidationContext<UpdateEstadoPagoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
