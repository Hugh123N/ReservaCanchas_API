using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Cancha.Ubigeo;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class DeleteUbigeoCommandValidator : CommandValidatorBase<DeleteUbigeoCommand>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _repositoryBase;
        public DeleteUbigeoCommandValidator(IRepository<Entity.Models.Ubigeo> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.Ubigeo.IdUbigeo)
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
