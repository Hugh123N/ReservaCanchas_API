using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class GetConfiguracionProveedorQueryValidator : QueryValidatorBase<GetConfiguracionProveedorQuery>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public GetConfiguracionProveedorQueryValidator(IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository)
        {
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;

            RequiredField(x => x.Id, Resources.Dbo.ConfiguracionProveedor.IdConfiguracionProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetConfiguracionProveedorQuery command, int id, ValidationContext<GetConfiguracionProveedorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ConfiguracionProveedorRepository.FindAll().Where(x => x.IdConfiguracionProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
