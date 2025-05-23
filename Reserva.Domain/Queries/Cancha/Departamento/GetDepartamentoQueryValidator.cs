using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class GetDepartamentoQueryValidator : QueryValidatorBase<GetDepartamentoQuery>
    {
        private readonly IRepository<Entity.Models.Departamento> _DepartamentoRepository;

        public GetDepartamentoQueryValidator(IRepository<Entity.Models.Departamento> DepartamentoRepository)
        {
            _DepartamentoRepository = DepartamentoRepository;

            RequiredField(x => x.Id, Resources.Cancha.Departamento.IdDepartamento)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDepartamentoQuery command, int id, ValidationContext<GetDepartamentoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _DepartamentoRepository.FindAll().Where(x => x.IdDepartamento == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
