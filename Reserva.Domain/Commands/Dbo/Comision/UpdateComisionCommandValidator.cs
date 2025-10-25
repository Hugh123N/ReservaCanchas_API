using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Comision
{
    public class UpdateComisionCommandValidator : CommandValidatorBase<UpdateComisionCommand>
    {
        private readonly IRepository<Entity.Comision> _repositoryBase;
        public UpdateComisionCommandValidator(IRepository<Entity.Comision> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdComision, Resources.Dbo.Comision.IdComision)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdComision)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Comision.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Comision.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateComisionCommand command, int id, ValidationContext<UpdateComisionCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdComision == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
