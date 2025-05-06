using FluentValidation;
using System.Linq.Expressions;
using Reserva.Dto.Base;

namespace Reserva.Domain.Queries.Base
{
    public class QueryValidatorBase<TRequest> : AbstractValidator<TRequest>
    {
        public bool CustomValidationMessage(ValidationContext<TRequest> context, string message)
        {
            context.MessageFormatter.AppendArgument("MensajePersonalizado", message);
            return false;
        }

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredInformation<TProperty>(
            Expression<Func<TRequest, TProperty>> expression,
            string? message = null
        )
            => RuleFor(expression).NotEmpty().WithMessage(message ?? "Este campo es obligatorio.");

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredField<TProperty>(
            Expression<Func<TRequest, TProperty>> expression,
            string field
        )
            => RuleFor(expression).NotEmpty().WithMessage($"El campo '{field}' es obligatorio.");
    }

    public static class BaseQueryValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithCustomValidationMessage<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule
        )
            => rule.WithMessage("Error de validación personalizada.");
    }
}
