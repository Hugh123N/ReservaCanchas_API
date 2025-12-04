using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class UpdateTipoSuperficieCommandValidator : CommandValidatorBase<UpdateTipoSuperficieCommand>
    {
        private readonly IRepository<Entity.TipoSuperficie> _repositoryBase;
        public UpdateTipoSuperficieCommandValidator(IRepository<Entity.TipoSuperficie> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdTipoSuperficie, Resources.Dbo.TipoSuperficie.IdTipoSuperficie)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdTipoSuperficie)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.TipoSuperficie.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.TipoSuperficie.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateTipoSuperficieCommand command, int id, ValidationContext<UpdateTipoSuperficieCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoSuperficie == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
