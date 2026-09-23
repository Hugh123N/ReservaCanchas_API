using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Operador;
using Reserva.Dto.Dbo.Servicio;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class GetOperadorQueryHandler : QueryHandlerBase<GetOperadorQuery, GetOperadorDto>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;

        public GetOperadorQueryHandler(
            IMapper mapper,
            GetOperadorQueryValidator validator,
            IRepository<Entity.Operador> OperadorRepository
        ) : base(mapper, validator)
        {
            _OperadorRepository = OperadorRepository;
        }

        protected override async Task<ResponseDto<GetOperadorDto>> HandleQuery(GetOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetOperadorDto>();
            var Operador = await _OperadorRepository.GetByAsync(x => x.IdOperador == request.Id, 
                x => x.IdUsuarioNavigation,
                x => x.OperadorCancha.Where(x => x.Activo));

            var operadorDto = _mapper?.Map<GetOperadorDto>(Operador);

            if (Operador != null && operadorDto != null)
            {
                operadorDto.Imagen = Operador?.IdUsuarioNavigation?.Imagen ?? string.Empty;
                operadorDto.FechaCreacion = Operador.CreateDate;
                operadorDto.FechaActualizacion = Operador.UpdateDate;
                operadorDto.CanchaIds = Operador.OperadorCancha.Select(oc => oc.IdCancha).ToList();

                response.UpdateData(operadorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
