using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class UpdateDetallePagoCommandValidator : CommandValidatorBase<UpdateDetallePagoCommand>
    {
        private readonly IRepository<Entity.Models.DetallePago> _repositoryBase;
        public UpdateDetallePagoCommandValidator(IRepository<Entity.Models.DetallePago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDetallePago, Resources.Cancha.DetallePago.IdDetallePago)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDetallePago)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.DetallePago.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.DetallePago.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDetallePagoCommand command, int id, ValidationContext<UpdateDetallePagoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDetallePago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
