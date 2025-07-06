using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class GetCanchaFavoritaQueryValidator : QueryValidatorBase<GetCanchaFavoritaQuery>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _CanchaFavoritaRepository;

        public GetCanchaFavoritaQueryValidator(IRepository<Entity.Models.CanchaFavorita> CanchaFavoritaRepository)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;

            RequiredField(x => x.Id, Resources.Cancha.CanchaFavorita.IdCanchaFavorita)
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
