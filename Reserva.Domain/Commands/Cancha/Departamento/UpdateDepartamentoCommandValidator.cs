using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class UpdateDepartamentoCommandValidator : CommandValidatorBase<UpdateDepartamentoCommand>
    {
        private readonly IRepository<Entity.Models.Departamento> _repositoryBase;
        public UpdateDepartamentoCommandValidator(IRepository<Entity.Models.Departamento> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDepartamento, Resources.Cancha.Departamento.IdDepartamento)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDepartamento)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Departamento.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Departamento.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDepartamentoCommand command, int id, ValidationContext<UpdateDepartamentoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDepartamento == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
