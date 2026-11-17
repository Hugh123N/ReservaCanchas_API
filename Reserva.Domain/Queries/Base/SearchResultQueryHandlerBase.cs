using AutoMapper;
using Reserva.Dto.Base;

namespace Reserva.Domain.Queries.Base;

public abstract class SearchQueryHandlerBase<TRequest, TFilter, TItem, TResult> : QueryHandlerBase<TRequest, TResult>
    where TRequest : SearchQueryBase<TFilter, TItem, TResult>
    where TResult : SearchResultDto<TItem>
{
    protected SearchQueryHandlerBase(IMapper mapper, QueryValidatorBase<TRequest> validator) : base(mapper, validator) { }

    protected override Task<ResponseDto<TResult>> ValidateAndHandle(TRequest request, CancellationToken cancellationToken)
    {
        _validator ??= new SearchQueryValidatorBase<TRequest, TFilter, TItem, TResult>();
        return base.ValidateAndHandle(request, cancellationToken);
    }
}
