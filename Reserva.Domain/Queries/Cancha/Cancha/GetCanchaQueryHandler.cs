using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Queries.Cancha.Disponibilidad;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class GetCanchaQueryHandler : QueryHandlerBase<GetCanchaQuery, GetCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public GetCanchaQueryHandler(
            IMapper mapper,
            IMediator mediator,
            GetCanchaQueryValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(mapper, mediator, validator)
        {
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<GetCanchaDto>> HandleQuery(GetCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.Id,
                x => x.IdTipoCanchaNavigation,
                x => x.ImagenCanchas.Where(i => i.Activo),
                x => x.IdEstadoCanchaNavigation,
                x => x.CanchaFavorita.Where(x => x.Activo),
                x => x.CodigoUbigeoNavigation!);

            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            var disponibilidad = await _mediator?.Send(new GetCanchaByFechaQuery(DateTime.Now, request.Id), cancellationToken)!;

            CanchaDto.Disponibilidad = disponibilidad.Data;
                
            if (Cancha != null && CanchaDto != null)
            {
                response.UpdateData(CanchaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
