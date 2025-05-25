using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class GetEstadoCanchaQueryHandler : QueryHandlerBase<GetEstadoCanchaQuery, GetEstadoCanchaDto>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public GetEstadoCanchaQueryHandler(
            IMapper mapper,
            GetEstadoCanchaQueryValidator validator,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(mapper, validator)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        protected override async Task<ResponseDto<GetEstadoCanchaDto>> HandleQuery(GetEstadoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoCanchaDto>();
            var EstadoCancha = await _EstadoCanchaRepository.GetByAsync(x => x.IdEstadoCancha == request.Id);
            var EstadoCanchaDto = _mapper?.Map<GetEstadoCanchaDto>(EstadoCancha);

            if (EstadoCancha != null && EstadoCanchaDto != null)
            {
                response.UpdateData(EstadoCanchaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
