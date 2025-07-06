using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class UpdateCanchaFavoritaCommandValidator : CommandValidatorBase<UpdateCanchaFavoritaCommand>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _repositoryBase;
        public UpdateCanchaFavoritaCommandValidator(IRepository<Entity.Models.CanchaFavorita> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCanchaFavorita, Resources.Cancha.CanchaFavorita.IdCanchaFavorita)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCanchaFavorita)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.CanchaFavorita.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.CanchaFavorita.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateCanchaFavoritaCommand command, int id, ValidationContext<UpdateCanchaFavoritaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
