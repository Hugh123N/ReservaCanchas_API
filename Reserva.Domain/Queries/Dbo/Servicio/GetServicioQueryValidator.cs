using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Servicio
{
    public class GetServicioQueryValidator : QueryValidatorBase<GetServicioQuery>
    {
        private readonly IRepository<Entity.Servicio> _ServicioRepository;

        public GetServicioQueryValidator(IRepository<Entity.Servicio> ServicioRepository)
        {
            _ServicioRepository = ServicioRepository;

            RequiredField(x => x.Id, Resources.Dbo.Servicio.IdServicio)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetServicioQuery command, int id, ValidationContext<GetServicioQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ServicioRepository.FindAll().Where(x => x.IdServicio == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
