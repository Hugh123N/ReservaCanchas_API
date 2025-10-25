using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Ubigeo;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class DeleteUbigeoCommandValidator : CommandValidatorBase<DeleteUbigeoCommand>
    {
        private readonly IRepository<Entity.Ubigeo> _repositoryBase;
        public DeleteUbigeoCommandValidator(IRepository<Entity.Ubigeo> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.Ubigeo.IdUbigeo)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteUbigeoCommand command, string id, ValidationContext<DeleteUbigeoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.CodigoUbigeo == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
