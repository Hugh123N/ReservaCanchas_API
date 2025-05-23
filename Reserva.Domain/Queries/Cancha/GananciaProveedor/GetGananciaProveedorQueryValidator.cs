using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class GetGananciaProveedorQueryValidator : QueryValidatorBase<GetGananciaProveedorQuery>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _GananciaProveedorRepository;

        public GetGananciaProveedorQueryValidator(IRepository<Entity.Models.GananciaProveedor> GananciaProveedorRepository)
        {
            _GananciaProveedorRepository = GananciaProveedorRepository;

            RequiredField(x => x.Id, Resources.Cancha.GananciaProveedor.IdGananciaProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetGananciaProveedorQuery command, int id, ValidationContext<GetGananciaProveedorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _GananciaProveedorRepository.FindAll().Where(x => x.IdGananciaProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
