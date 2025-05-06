using AutoMapper;
using MediatR;
using Reserva.Dto.Base;


namespace Reserva.Domain.Queries.Base
{
    public abstract class SearchQueryHandlerBase<TRequest, TFilter, TResponse> : QueryHandlerBase<TRequest, SearchResultDto<TResponse>>
         where TRequest : SearchQueryBase<TFilter, TResponse>
    {
        public SearchQueryHandlerBase()
        {

        }

        public SearchQueryHandlerBase(IMapper mapper) : base(mapper) { }

        public SearchQueryHandlerBase(IMapper mapper, IMediator mediator) : base(mapper, mediator) { }

        public SearchQueryHandlerBase(IMapper mapper, QueryValidatorBase<TRequest> validator) : base(mapper, validator) { }

        public SearchQueryHandlerBase(IMapper mapper, IMediator mediator, QueryValidatorBase<TRequest> validator) : base(mapper, mediator, validator) { }

        protected override async Task<ResponseDto<SearchResultDto<TResponse>>> ValidateAndHandle(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator == null)
                _validator = new SearchQueryValidatorBase<TRequest, TFilter, TResponse>();

            return await base.ValidateAndHandle(request, cancellationToken);
        }
    }
}
