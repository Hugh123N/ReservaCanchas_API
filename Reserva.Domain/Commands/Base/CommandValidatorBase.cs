using FluentValidation;
using System.Linq.Expressions;

namespace Reserva.Domain.Commands.Base
{

    public class CommandValidatorBase<TRequest> : AbstractValidator<TRequest>
    {
        public bool CustomValidationMessage(ValidationContext<TRequest> context, string message)
        {
            context.MessageFormatter.AppendArgument("MensajePersonalizado", message);
            return false;
        }

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredInformation<TProperty>(Expression<Func<TRequest, TProperty>> expression, string? message = null)
            => RuleFor(expression).NotEmpty().WithMessage(message ?? "La información es requerida.");

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredField<TProperty>(Expression<Func<TRequest, TProperty>> expression, string field)
            => RuleFor(expression).NotEmpty().WithMessage($"El campo '{field}' es obligatorio.");

        protected IRuleBuilderOptions<TRequest, string?> RequiredField(Expression<Func<TRequest, string?>> expression, string field)
            => RuleFor(expression).NotEmpty().WithMessage($"El campo '{field}' es obligatorio.");

        protected IRuleBuilderOptions<TRequest, string?> MinimumLength(Expression<Func<TRequest, string?>> expression, string field, int minimumLength)
            => RuleFor(expression).MinimumLength(minimumLength).WithMessage($"El campo '{field}' debe tener al menos {minimumLength} caracteres.");

        protected IRuleBuilderOptions<TRequest, string?> MaximumLength(Expression<Func<TRequest, string?>> expression, string field, int maximumLength)
            => RuleFor(expression).MaximumLength(maximumLength).WithMessage($"El campo '{field}' no debe exceder los {maximumLength} caracteres.");

        protected IRuleBuilderOptions<TRequest, string?> RequiredString(Expression<Func<TRequest, string?>> expression, string field, int? minimumLength, int? maximumLength)
        {
            var ruleBuilderOptions = RequiredField(expression, field);

            if (minimumLength.HasValue || maximumLength.HasValue)
            {
                ruleBuilderOptions = ruleBuilderOptions.DependentRules(() =>
                {
                    if (minimumLength.HasValue)
                        ruleBuilderOptions = MinimumLength(expression, field, minimumLength.Value);

                    if (maximumLength.HasValue)
                        ruleBuilderOptions = MaximumLength(expression, field, maximumLength.Value);
                });
            }

            return ruleBuilderOptions;
        }
    }

    public static class BaseCommandValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithCustomValidationMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule)
            => rule.WithMessage("Validación personalizada no cumplida.");
    }
}
