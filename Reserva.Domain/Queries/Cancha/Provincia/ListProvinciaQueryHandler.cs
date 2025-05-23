using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class ListProvinciaQueryHandler : QueryHandlerBase<ListProvinciaQuery, IEnumerable<ListProvinciaDto>>
    {
        private readonly IRepository<Entity.Models.Provincia> _repository;

        public ListProvinciaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Provincia> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListProvinciaDto>>> HandleQuery(ListProvinciaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListProvinciaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdProvincia == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListProvinciaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListProvinciaDto>());

            return await Task.FromResult(response);
        }
    }
}
