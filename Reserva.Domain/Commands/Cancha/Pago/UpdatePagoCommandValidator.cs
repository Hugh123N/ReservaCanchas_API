using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Pago
{
    public class UpdatePagoCommandValidator : CommandValidatorBase<UpdatePagoCommand>
    {
        private readonly IRepository<Entity.Models.Pago> _repositoryBase;
        public UpdatePagoCommandValidator(IRepository<Entity.Models.Pago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdPago, Resources.Cancha.Pago.IdPago)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdPago)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Pago.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Pago.FechaIngreso);
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
