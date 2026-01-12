using FluentValidation;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    /// <summary>
    /// Validador para GetCanchasUsuarioQuery
    /// </summary>
    public class GetCanchasUsuarioQueryValidator : QueryValidatorBase<GetCanchasUsuarioQuery>
    {
        public GetCanchasUsuarioQueryValidator()
        {
            /*RuleFor(x => x.Roles)
                .NotEmpty()
                .WithMessage("Debe proporcionar al menos un rol");

            RuleFor(x => x.Roles)
                .Must(roles => roles != null && roles.Any())
                .WithMessage("La lista de roles no puede estar vacía");*/
        }
    }
}
