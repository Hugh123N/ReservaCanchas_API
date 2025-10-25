using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class UpdatePagoCommandValidator : CommandValidatorBase<UpdatePagoCommand>
    {
        private readonly IRepository<Entity.Pago> _repositoryBase;
        public UpdatePagoCommandValidator(IRepository<Entity.Pago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdPago, Resources.Dbo.Pago.IdPago)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdPago)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Pago.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Pago.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdatePagoCommand command, int id, ValidationContext<UpdatePagoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
