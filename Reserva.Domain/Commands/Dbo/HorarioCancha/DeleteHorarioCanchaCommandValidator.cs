using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class DeleteHorarioCanchaCommandValidator : CommandValidatorBase<DeleteHorarioCanchaCommand>
    {
        private readonly IRepository<Entity.HorarioCancha> _repositoryBase;
        public DeleteHorarioCanchaCommandValidator(IRepository<Entity.HorarioCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.HorarioCancha.IdHorarioCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteHorarioCanchaCommand command, int id, ValidationContext<DeleteHorarioCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdHorarioCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
