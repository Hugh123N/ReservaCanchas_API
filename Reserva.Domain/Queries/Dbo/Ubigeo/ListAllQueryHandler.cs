using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Ubigeo;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Ubigeo
{
    public class ListAllQueryHandler : QueryHandlerBase<ListAllQuery, IEnumerable<GetUbigeoDto>>
    {
        private readonly IRepository<Entity.Ubigeo> _repository;

        public ListAllQueryHandler(
            IMapper mapper,
            IRepository<Entity.Ubigeo> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<GetUbigeoDto>>> HandleQuery(ListAllQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<GetUbigeoDto>>();
            var ubigeos = await _repository.FindAllAsNoTracking().ToListAsync();

            var listDtos = _mapper?.Map<IEnumerable<GetUbigeoDto>>(ubigeos);

            response.UpdateData(listDtos ?? new List<GetUbigeoDto>());

            return response;
        }
    }
}
