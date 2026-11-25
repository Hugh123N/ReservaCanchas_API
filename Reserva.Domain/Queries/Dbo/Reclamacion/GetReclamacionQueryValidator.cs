using FluentValidation;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Reclamacion
{
    public class GetReclamacionQueryValidator : QueryValidatorBase<GetReclamacionQuery>
    {
        public GetReclamacionQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID de la reclamación es requerido.");
        }
    }
}
