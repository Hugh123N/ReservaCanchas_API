using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class UpdateHorarioCanchaCommandValidator : CommandValidatorBase<UpdateHorarioCanchaCommand>
    {
        private readonly IRepository<Entity.HorarioCancha> _repositoryBase;
        public UpdateHorarioCanchaCommandValidator(IRepository<Entity.HorarioCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdHorarioCancha, Resources.Dbo.HorarioCancha.IdHorarioCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdHorarioCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.HorarioCancha.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.HorarioCancha.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateHorarioCanchaCommand command, int id, ValidationContext<UpdateHorarioCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdHorarioCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
