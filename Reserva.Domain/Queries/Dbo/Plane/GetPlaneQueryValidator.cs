using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Plane
{
    public class GetPlaneQueryValidator : QueryValidatorBase<GetPlaneQuery>
    {
        private readonly IRepository<Entity.Plane> _PlaneRepository;

        public GetPlaneQueryValidator(IRepository<Entity.Plane> PlaneRepository)
        {
            _PlaneRepository = PlaneRepository;

            RequiredField(x => x.Id, Resources.Dbo.Plane.IdPlane)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetPlaneQuery command, int id, ValidationContext<GetPlaneQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _PlaneRepository.FindAll().Where(x => x.IdPlane == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
