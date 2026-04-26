using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class UpdatePagoPlanCommandValidator : CommandValidatorBase<UpdatePagoPlanCommand>
    {
        private readonly IRepository<Entity.PagoPlan> _repositoryBase;
        public UpdatePagoPlanCommandValidator(IRepository<Entity.PagoPlan> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdPagoPlan, Resources.Dbo.PagoPlan.IdPagoPlan)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdPagoPlan)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.PagoPlan.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.PagoPlan.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdatePagoPlanCommand command, int id, ValidationContext<UpdatePagoPlanCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdPagoPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
