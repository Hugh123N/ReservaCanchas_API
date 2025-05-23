using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class UpdateProvinciaCommandValidator : CommandValidatorBase<UpdateProvinciaCommand>
    {
        private readonly IRepository<Entity.Models.Provincia> _repositoryBase;
        public UpdateProvinciaCommandValidator(IRepository<Entity.Models.Provincia> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdProvincia, Resources.Cancha.Provincia.IdProvincia)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdProvincia)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Provincia.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Provincia.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateProvinciaCommand command, int id, ValidationContext<UpdateProvinciaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdProvincia == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
