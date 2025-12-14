using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class GetOperadorQueryValidator : QueryValidatorBase<GetOperadorQuery>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;

        public GetOperadorQueryValidator(IRepository<Entity.Operador> OperadorRepository)
        {
            _OperadorRepository = OperadorRepository;

            RequiredField(x => x.Id, Resources.Dbo.Operador.IdOperador)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetOperadorQuery command, int id, ValidationContext<GetOperadorQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _OperadorRepository.FindAll().Where(x => x.IdOperador == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
