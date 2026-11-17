using Reserva.Dto.Base;

namespace Reserva.Domain.Queries.Base
{
    public class SearchQueryBase<TFilter, TResponse> : SearchQueryBase<TFilter, TResponse, SearchResultDto<TResponse>>
    {
        public SearchQueryBase(SearchParamsDto<TFilter> searchParams) : base(searchParams) { }
    }

    public class SearchQueryBase<TFilter, TResponse, TResult> : QueryBase<TResult> where TResult : SearchResultDto<TResponse>
    {
        public SearchQueryBase(SearchParamsDto<TFilter> searchParams) => SearchParams = searchParams;
        public SearchParamsDto<TFilter> SearchParams { get; set; }
    }
}
