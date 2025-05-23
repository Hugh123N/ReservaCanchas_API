using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class GetProveedorQueryValidator : QueryValidatorBase<GetProveedorQuery>
    {
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;

        public GetProveedorQueryValidator(IRepository<Entity.Models.Proveedor> ProveedorRepository)
        {
            _ProveedorRepository = ProveedorRepository;

            RequiredField(x => x.Id, Resources.Cancha.Proveedor.IdProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetProveedorQuery command, int id, ValidationContext<GetProveedorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ProveedorRepository.FindAll().Where(x => x.IdProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
