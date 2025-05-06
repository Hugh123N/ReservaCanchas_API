using FluentValidation;

namespace Reserva.Domain.Queries.Base
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class SearchQueryValidatorBase<TRequest, TFilter, TResponse> : QueryValidatorBase<TRequest>
        where TRequest : SearchQueryBase<TFilter, TResponse>
    {
        private readonly string[] SortDirenctions = new string[] { "asc", "desc" };

        public SearchQueryValidatorBase()
        {
            RequiredInformation(x => x.SearchParams, "La información de búsqueda es obligatoria.")
                .DependentRules(() =>
                {
                    RequiredInformation(x => x.SearchParams.Page, "La información de la página es obligatoria.")
                        .DependentRules(() =>
                        {
                            RequiredField(x => x.SearchParams.Page.Page, "Página")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.SearchParams.Page.Page)
                                        .GreaterThan(0)
                                        .WithMessage("La página debe ser mayor a 0.");
                                });

                            RequiredField(x => x.SearchParams.Page.PageSize, "Tamaño de página")
                                .DependentRules(() =>
                                {
                                    RequiredField(x => x.SearchParams.Page.PageSize, "Tamaño de página")
                                        .GreaterThan(0)
                                        .WithMessage("El tamaño de página debe ser mayor a 0.");
                                });
                        });

                    RuleFor(x => x.SearchParams.Sort).Must((request, sort, context) =>
                    {
                        if (sort == null) return true;
                        if (!sort.Any()) return true;

                        var invalidSortDirs = sort.Where(x => !SortDirenctions.Contains(x.Direction));
                        if (invalidSortDirs.Any())
                            return CustomValidationMessage(context, "La dirección de ordenamiento no es válida (debe ser 'asc' o 'desc').");

                        return true;
                    }).WithMessage("La dirección de ordenamiento no es válida.");
                });
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
