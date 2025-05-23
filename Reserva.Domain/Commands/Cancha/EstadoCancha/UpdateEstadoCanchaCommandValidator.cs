using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class UpdateEstadoCanchaCommandValidator : CommandValidatorBase<UpdateEstadoCanchaCommand>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _repositoryBase;
        public UpdateEstadoCanchaCommandValidator(IRepository<Entity.Models.EstadoCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdEstadoCancha, Resources.Cancha.EstadoCancha.IdEstadoCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdEstadoCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.EstadoCancha.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.EstadoCancha.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateEstadoCanchaCommand command, int id, ValidationContext<UpdateEstadoCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
