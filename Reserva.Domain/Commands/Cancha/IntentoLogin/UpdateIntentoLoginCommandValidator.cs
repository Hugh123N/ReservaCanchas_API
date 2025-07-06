using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class UpdateIntentoLoginCommandValidator : CommandValidatorBase<UpdateIntentoLoginCommand>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _repositoryBase;
        public UpdateIntentoLoginCommandValidator(IRepository<Entity.Models.IntentoLogin> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdIntentoLogin, "El campo 'IdIntentoLogin' es obligatorio.")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdIntentoLogin)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.IntentoLogin.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.IntentoLogin.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateIntentoLoginCommand command, int id, ValidationContext<UpdateIntentoLoginCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdIntentoLogin == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
