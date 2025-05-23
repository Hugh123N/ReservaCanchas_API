using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class DeleteDepartamentoCommandValidator : CommandValidatorBase<DeleteDepartamentoCommand>
    {
        private readonly IRepository<Entity.Models.Departamento> _repositoryBase;
        public DeleteDepartamentoCommandValidator(IRepository<Entity.Models.Departamento> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.Departamento.IdDepartamento)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteDepartamentoCommand command, int id, ValidationContext<DeleteDepartamentoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDepartamento == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
