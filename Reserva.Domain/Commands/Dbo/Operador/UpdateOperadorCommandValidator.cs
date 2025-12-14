using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class UpdateOperadorCommandValidator : CommandValidatorBase<UpdateOperadorCommand>
    {
        private readonly IRepository<Entity.Operador> _repositoryBase;
        public UpdateOperadorCommandValidator(IRepository<Entity.Operador> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdOperador, Resources.Dbo.Operador.IdOperador)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdOperador)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Operador.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Operador.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateOperadorCommand command, int id, ValidationContext<UpdateOperadorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdOperador == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
