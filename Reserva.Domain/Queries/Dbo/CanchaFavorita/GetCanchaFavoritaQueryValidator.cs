using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class GetCanchaFavoritaQueryValidator : QueryValidatorBase<GetCanchaFavoritaQuery>
    {
        private readonly IRepository<Entity.CanchaFavorita> _CanchaFavoritaRepository;

        public GetCanchaFavoritaQueryValidator(IRepository<Entity.CanchaFavorita> CanchaFavoritaRepository)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;

            RequiredField(x => x.Id, Resources.Dbo.CanchaFavorita.IdCanchaFavorita)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetCanchaFavoritaQuery command, int id, ValidationContext<GetCanchaFavoritaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _CanchaFavoritaRepository.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
