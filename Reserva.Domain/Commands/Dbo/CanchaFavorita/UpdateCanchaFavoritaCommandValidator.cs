using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.CanchaFavorita
{
    public class UpdateCanchaFavoritaCommandValidator : CommandValidatorBase<UpdateCanchaFavoritaCommand>
    {
        private readonly IRepository<Entity.CanchaFavorita> _repositoryBase;
        public UpdateCanchaFavoritaCommandValidator(IRepository<Entity.CanchaFavorita> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCanchaFavorita, Resources.Dbo.CanchaFavorita.IdCanchaFavorita)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCanchaFavorita)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.CanchaFavorita.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.CanchaFavorita.FechaIngreso);
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
