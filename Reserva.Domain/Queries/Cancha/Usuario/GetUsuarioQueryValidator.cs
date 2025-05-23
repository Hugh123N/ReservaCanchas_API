using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class GetUsuarioQueryValidator : QueryValidatorBase<GetUsuarioQuery>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;

        public GetUsuarioQueryValidator(IRepository<Entity.Models.Usuario> UsuarioRepository)
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

        protected async Task<bool> ValidateExistenceAsync(GetUsuarioQuery command, int id, ValidationContext<GetUsuarioQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _UsuarioRepository.FindAll().Where(x => x.IdUsuario == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
