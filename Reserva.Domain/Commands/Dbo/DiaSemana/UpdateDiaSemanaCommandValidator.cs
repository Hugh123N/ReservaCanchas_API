using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class UpdateDiaSemanaCommandValidator : CommandValidatorBase<UpdateDiaSemanaCommand>
    {
        private readonly IRepository<Entity.DiaSemana> _repositoryBase;
        public UpdateDiaSemanaCommandValidator(IRepository<Entity.DiaSemana> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDiaSemana, Resources.Dbo.DiaSemana.IdDiaSemana)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDiaSemana)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.DiaSemana.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.DiaSemana.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDiaSemanaCommand command, int id, ValidationContext<UpdateDiaSemanaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDiaSemana == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
