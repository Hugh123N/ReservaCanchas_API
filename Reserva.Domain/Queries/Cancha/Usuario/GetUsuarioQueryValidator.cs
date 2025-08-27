using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class GetUsuarioQueryValidator : QueryValidatorBase<GetUsuarioQuery>
    {
        private readonly IRepository<Entity.AspNetUsers> _UsuarioRepository;

        public GetUsuarioQueryValidator(IRepository<Entity.AspNetUsers> UsuarioRepository)
        {
            _UsuarioRepository = UsuarioRepository;

            RequiredField(x => x.Id, Resources.Cancha.Usuario.IdUsuario)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetUsuarioQuery command, Guid id, ValidationContext<GetUsuarioQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _UsuarioRepository.FindAll().Where(x => x.Id == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
