using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class GetRolQueryValidator : QueryValidatorBase<GetRolQuery>
    {
        private readonly IRepository<Entity.Models.Rol> _RolRepository;

        public GetRolQueryValidator(IRepository<Entity.Models.Rol> RolRepository)
        {
            _RolRepository = RolRepository;

            RequiredField(x => x.Id, Resources.Cancha.Rol.IdRol)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetRolQuery command, int id, ValidationContext<GetRolQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _RolRepository.FindAll().Where(x => x.IdRol == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
