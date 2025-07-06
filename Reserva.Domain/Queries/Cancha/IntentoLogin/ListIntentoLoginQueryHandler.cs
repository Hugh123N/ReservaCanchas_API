using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.IntentoLogin;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class ListIntentoLoginQueryHandler : QueryHandlerBase<ListIntentoLoginQuery, IEnumerable<ListIntentoLoginDto>>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _repository;

        public ListIntentoLoginQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.IntentoLogin> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListIntentoLoginDto>>> HandleQuery(ListIntentoLoginQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListIntentoLoginDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdIntentoLogin == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListIntentoLoginDto>>(list);

            response.UpdateData(listDtos ?? new List<ListIntentoLoginDto>());

            return await Task.FromResult(response);
        }
    }
}
