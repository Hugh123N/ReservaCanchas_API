using FluentValidation;

namespace Reserva.Domain.Queries.Base
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class SearchQueryValidatorBase<TRequest, TFilter, TResponse> : QueryValidatorBase<TRequest>
        where TRequest : SearchQueryBase<TFilter, TResponse>
    {
        private readonly SearchQueryValidatorBase<TRequest, TFilter, TResponse, Dto.Base.SearchResultDto<TResponse>> _common = new();

        public SearchQueryValidatorBase()
        {
            Include(_common);
        }
    }

    public class SearchQueryValidatorBase<TRequest, TFilter, TResponse, TResult> : QueryValidatorBase<TRequest>
        where TRequest : SearchQueryBase<TFilter, TResponse, TResult>
        where TResult : Dto.Base.SearchResultDto<TResponse>
    {
        private readonly string[] SortDirenctions = new string[] { "asc", "desc" };

        public SearchQueryValidatorBase()
        {
            RequiredInformation(x => x.SearchParams, "La informaci�n de b�squeda es obligatoria.")
                .DependentRules(() =>
                {
                    RequiredInformation(x => x.SearchParams.Page, "La informaci�n de la p�gina es obligatoria.")
                        .DependentRules(() =>
                        {
                            RequiredField(x => x.SearchParams.Page.Page, "P�gina")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.SearchParams.Page.Page)
                                        .GreaterThan(0)
                                        .WithMessage("La p�gina debe ser mayor a 0.");
                                });

                            RequiredField(x => x.SearchParams.Page.PageSize, "Tama�o de p�gina")
                                .DependentRules(() =>
                                {
                                    RequiredField(x => x.SearchParams.Page.PageSize, "Tama�o de p�gina")
                                        .GreaterThan(0)
                                        .WithMessage("El tama�o de p�gina debe ser mayor a 0.");
                                });
                        });

                    RuleFor(x => x.SearchParams.Sort).Must((request, sort, context) =>
                    {
                        if (sort == null) return true;
                        if (!sort.Any()) return true;

                        var invalidSortDirs = sort.Where(x => !SortDirenctions.Contains(x.Direction));
                        if (invalidSortDirs.Any())
                            return CustomValidationMessage(context, "La direcci�n de ordenamiento no es v�lida (debe ser 'asc' o 'desc').");

                        return true;
                    }).WithMessage("La direcci�n de ordenamiento no es v�lida.");
                });
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
