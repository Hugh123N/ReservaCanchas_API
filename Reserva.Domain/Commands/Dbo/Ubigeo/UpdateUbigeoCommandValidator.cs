using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class UpdateUbigeoCommandValidator : CommandValidatorBase<UpdateUbigeoCommand>
    {
        private readonly IRepository<Entity.Ubigeo> _repositoryBase;
        public UpdateUbigeoCommandValidator(IRepository<Entity.Ubigeo> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.CodigoUbigeo, Resources.Dbo.Ubigeo.IdUbigeo)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.CodigoUbigeo)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Ubigeo.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Ubigeo.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateUbigeoCommand command, string id, ValidationContext<UpdateUbigeoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.CodigoUbigeo == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
