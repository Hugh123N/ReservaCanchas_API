using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class UpdateTipoCanchaCommandValidator : CommandValidatorBase<UpdateTipoCanchaCommand>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _repositoryBase;
        public UpdateTipoCanchaCommandValidator(IRepository<Entity.Models.TipoCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdTipoCancha, Resources.Cancha.TipoCancha.IdTipoCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdTipoCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.TipoCancha.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.TipoCancha.FechaIngreso);
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
