using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class GetTipoCanchaQueryValidator : QueryValidatorBase<GetTipoCanchaQuery>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _TipoCanchaRepository;

        public GetTipoCanchaQueryValidator(IRepository<Entity.Models.TipoCancha> TipoCanchaRepository)
        {
            _TipoCanchaRepository = TipoCanchaRepository;

            RequiredField(x => x.Id, Resources.Cancha.TipoCancha.IdTipoCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetTipoCanchaQuery command, int id, ValidationContext<GetTipoCanchaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _TipoCanchaRepository.FindAll().Where(x => x.IdTipoCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
