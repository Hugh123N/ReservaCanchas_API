using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class GetTipoProveedorQueryValidator : QueryValidatorBase<GetTipoProveedorQuery>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _TipoProveedorRepository;

        public GetTipoProveedorQueryValidator(IRepository<Entity.Models.TipoProveedor> TipoProveedorRepository)
        {
            _TipoProveedorRepository = TipoProveedorRepository;

            RequiredField(x => x.Id, Resources.Cancha.TipoProveedor.IdTipoProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetTipoProveedorQuery command, int id, ValidationContext<GetTipoProveedorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _TipoProveedorRepository.FindAll().Where(x => x.IdTipoProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
