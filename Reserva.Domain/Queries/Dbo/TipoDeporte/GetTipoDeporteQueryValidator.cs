using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoDeporte
{
    public class GetTipoDeporteQueryValidator : QueryValidatorBase<GetTipoDeporteQuery>
    {
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;

        public GetTipoDeporteQueryValidator(IRepository<Entity.TipoDeporte> TipoDeporteRepository)
        {
            _TipoDeporteRepository = TipoDeporteRepository;

            RequiredField(x => x.Id, Resources.Dbo.TipoDeporte.IdTipoDeporte)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetTipoDeporteQuery command, int id, ValidationContext<GetTipoDeporteQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _TipoDeporteRepository.FindAll().Where(x => x.IdTipoDeporte == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
