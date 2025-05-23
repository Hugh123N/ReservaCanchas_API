using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class ListImagenCanchaQueryHandler : QueryHandlerBase<ListImagenCanchaQuery, IEnumerable<ListImagenCanchaDto>>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _repository;

        public ListImagenCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.ImagenCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListImagenCanchaDto>>> HandleQuery(ListImagenCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListImagenCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdImagenCancha == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListImagenCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListImagenCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
