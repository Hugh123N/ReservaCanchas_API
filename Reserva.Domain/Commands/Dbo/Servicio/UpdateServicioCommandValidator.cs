using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class UpdateServicioCommandValidator : CommandValidatorBase<UpdateServicioCommand>
    {
        private readonly IRepository<Entity.Servicio> _repositoryBase;
        public UpdateServicioCommandValidator(IRepository<Entity.Servicio> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdServicio, Resources.Dbo.Servicio.IdServicio)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdServicio)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Servicio.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Servicio.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateServicioCommand command, int id, ValidationContext<UpdateServicioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdServicio == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
