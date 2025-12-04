using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoSuperficie
{
    public class GetTipoSuperficieQueryValidator : QueryValidatorBase<GetTipoSuperficieQuery>
    {
        private readonly IRepository<Entity.TipoSuperficie> _TipoSuperficieRepository;

        public GetTipoSuperficieQueryValidator(IRepository<Entity.TipoSuperficie> TipoSuperficieRepository)
        {
            _TipoSuperficieRepository = TipoSuperficieRepository;

            RequiredField(x => x.Id, Resources.Dbo.TipoSuperficie.IdTipoSuperficie)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetTipoSuperficieQuery command, int id, ValidationContext<GetTipoSuperficieQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _TipoSuperficieRepository.FindAll().Where(x => x.IdTipoSuperficie == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
