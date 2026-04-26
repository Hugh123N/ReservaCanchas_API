using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class UpdatePlaneCommandValidator : CommandValidatorBase<UpdatePlaneCommand>
    {
        private readonly IRepository<Entity.Plane> _repositoryBase;
        public UpdatePlaneCommandValidator(IRepository<Entity.Plane> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdPlane, Resources.Dbo.Plane.IdPlane)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdPlane)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Plane.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Plane.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdatePlaneCommand command, int id, ValidationContext<UpdatePlaneCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdPlane == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
