using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class GetEstadoUsuarioQueryValidator : QueryValidatorBase<GetEstadoUsuarioQuery>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public GetEstadoUsuarioQueryValidator(IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository)
        {
            _EstadoUsuarioRepository = EstadoUsuarioRepository;

            RequiredField(x => x.Id, Resources.Cancha.EstadoUsuario.IdEstadoUsuario)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetEstadoUsuarioQuery command, int id, ValidationContext<GetEstadoUsuarioQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _EstadoUsuarioRepository.FindAll().Where(x => x.IdEstadoUsuario == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
