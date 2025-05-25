using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class UpdateUbigeoCommandValidator : CommandValidatorBase<UpdateUbigeoCommand>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _repositoryBase;
        public UpdateUbigeoCommandValidator(IRepository<Entity.Models.Ubigeo> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.CodigoUbigeo, Resources.Cancha.Ubigeo.IdUbigeo)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.CodigoUbigeo)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Ubigeo.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Ubigeo.FechaIngreso);
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
