using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class UpdateDistritoCommandValidator : CommandValidatorBase<UpdateDistritoCommand>
    {
        private readonly IRepository<Entity.Models.Distrito> _repositoryBase;
        public UpdateDistritoCommandValidator(IRepository<Entity.Models.Distrito> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDistrito, Resources.Cancha.Distrito.IdDistrito)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDistrito)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Distrito.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Distrito.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDistritoCommand command, int id, ValidationContext<UpdateDistritoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDistrito == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
