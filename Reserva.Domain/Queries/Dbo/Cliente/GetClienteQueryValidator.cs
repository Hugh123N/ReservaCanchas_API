using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Cliente
{
    public class GetClienteQueryValidator : QueryValidatorBase<GetClienteQuery>
    {
        private readonly IRepository<Entity.Cliente> _ClienteRepository;

        public GetClienteQueryValidator(IRepository<Entity.Cliente> ClienteRepository)
        {
            _ClienteRepository = ClienteRepository;

            RequiredField(x => x.Id, "IdCliente")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetClienteQuery command, int id, ValidationContext<GetClienteQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ClienteRepository.FindAll().Where(x => x.IdCliente == id && x.Activo).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
