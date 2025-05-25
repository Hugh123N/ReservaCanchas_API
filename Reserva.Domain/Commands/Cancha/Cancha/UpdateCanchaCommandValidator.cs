using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class UpdateCanchaCommandValidator : CommandValidatorBase<UpdateCanchaCommand>
    {
        private readonly IRepository<Entity.Models.Cancha> _repositoryBase;
        public UpdateCanchaCommandValidator(IRepository<Entity.Models.Cancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCancha, Resources.Cancha.Cancha.IdCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Cancha.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Cancha.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateCanchaCommand command, int id, ValidationContext<UpdateCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
