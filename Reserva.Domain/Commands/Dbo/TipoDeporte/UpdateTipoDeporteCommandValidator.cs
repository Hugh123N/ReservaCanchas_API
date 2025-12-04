using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class UpdateTipoDeporteCommandValidator : CommandValidatorBase<UpdateTipoDeporteCommand>
    {
        private readonly IRepository<Entity.TipoDeporte> _repositoryBase;
        public UpdateTipoDeporteCommandValidator(IRepository<Entity.TipoDeporte> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdTipoDeporte, Resources.Dbo.TipoDeporte.IdTipoDeporte)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdTipoDeporte)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.TipoDeporte.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.TipoDeporte.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateTipoDeporteCommand command, int id, ValidationContext<UpdateTipoDeporteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoDeporte == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
