using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class UpdateDetalleReservaCommandValidator : CommandValidatorBase<UpdateDetalleReservaCommand>
    {
        private readonly IRepository<Entity.DetalleReserva> _repositoryBase;
        public UpdateDetalleReservaCommandValidator(IRepository<Entity.DetalleReserva> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDetalleReserva, Resources.Dbo.DetalleReserva.IdDetalleReserva)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDetalleReserva)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.DetalleReserva.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.DetalleReserva.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDetalleReservaCommand command, int id, ValidationContext<UpdateDetalleReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDetalleReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
