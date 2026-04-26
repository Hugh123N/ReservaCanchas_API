using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class UpdateComprobantePagoPlanCommandValidator : CommandValidatorBase<UpdateComprobantePagoPlanCommand>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _repositoryBase;
        public UpdateComprobantePagoPlanCommandValidator(IRepository<Entity.ComprobantePagoPlan> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdComprobantePagoPlan, Resources.Dbo.ComprobantePagoPlan.IdComprobantePagoPlan)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdComprobantePagoPlan)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.ComprobantePagoPlan.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.ComprobantePagoPlan.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateComprobantePagoPlanCommand command, int id, ValidationContext<UpdateComprobantePagoPlanCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdComprobantePagoPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
