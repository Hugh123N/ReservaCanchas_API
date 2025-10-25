using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoCancha
{
    public class UpdateTipoCanchaCommandValidator : CommandValidatorBase<UpdateTipoCanchaCommand>
    {
        private readonly IRepository<Entity.TipoCancha> _repositoryBase;
        public UpdateTipoCanchaCommandValidator(IRepository<Entity.TipoCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdTipoCancha, Resources.Dbo.TipoCancha.IdTipoCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdTipoCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.TipoCancha.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.TipoCancha.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateTipoCanchaCommand command, int id, ValidationContext<UpdateTipoCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
