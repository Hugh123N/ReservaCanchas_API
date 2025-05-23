using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class GetImagenCanchaQueryValidator : QueryValidatorBase<GetImagenCanchaQuery>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _ImagenCanchaRepository;

        public GetImagenCanchaQueryValidator(IRepository<Entity.Models.ImagenCancha> ImagenCanchaRepository)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;

            RequiredField(x => x.Id, Resources.Cancha.ImagenCancha.IdImagenCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetImagenCanchaQuery command, int id, ValidationContext<GetImagenCanchaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ImagenCanchaRepository.FindAll().Where(x => x.IdImagenCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
