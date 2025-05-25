using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class GetEstadoProveedorQueryValidator : QueryValidatorBase<GetEstadoProveedorQuery>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;

        public GetEstadoProveedorQueryValidator(IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository)
        {
            _EstadoProveedorRepository = EstadoProveedorRepository;

            RequiredField(x => x.Id, Resources.Cancha.EstadoProveedor.IdEstadoProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetEstadoProveedorQuery command, int id, ValidationContext<GetEstadoProveedorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _EstadoProveedorRepository.FindAll().Where(x => x.IdEstadoProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
