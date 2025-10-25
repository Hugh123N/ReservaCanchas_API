using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Proveedor
{
    public class GetProveedorQueryValidator : QueryValidatorBase<GetProveedorQuery>
    {
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;

        public GetProveedorQueryValidator(IRepository<Entity.Proveedor> ProveedorRepository)
        {
            _ProveedorRepository = ProveedorRepository;

            RequiredField(x => x.Id, Resources.Dbo.Proveedor.IdProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetProveedorQuery command, Guid id, ValidationContext<GetProveedorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ProveedorRepository.FindAll().Where(x => x.IdProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
