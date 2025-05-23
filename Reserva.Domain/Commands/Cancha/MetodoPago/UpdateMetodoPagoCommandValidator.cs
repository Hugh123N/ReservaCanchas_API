using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class UpdateMetodoPagoCommandValidator : CommandValidatorBase<UpdateMetodoPagoCommand>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _repositoryBase;
        public UpdateMetodoPagoCommandValidator(IRepository<Entity.Models.MetodoPago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdMetodoPago, Resources.Cancha.MetodoPago.IdMetodoPago)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdMetodoPago)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.MetodoPago.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.MetodoPago.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateMetodoPagoCommand command, int id, ValidationContext<UpdateMetodoPagoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdMetodoPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
