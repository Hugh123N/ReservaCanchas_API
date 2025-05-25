using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoUsuario;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class GetEstadoUsuarioQueryHandler : QueryHandlerBase<GetEstadoUsuarioQuery, GetEstadoUsuarioDto>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public GetEstadoUsuarioQueryHandler(
            IMapper mapper,
            GetEstadoUsuarioQueryValidator validator,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository
        ) : base(mapper, validator)
        {
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        protected override async Task<ResponseDto<GetEstadoUsuarioDto>> HandleQuery(GetEstadoUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoUsuarioDto>();
            var EstadoUsuario = await _EstadoUsuarioRepository.GetByAsync(x => x.IdEstadoUsuario == request.Id);
            var EstadoUsuarioDto = _mapper?.Map<GetEstadoUsuarioDto>(EstadoUsuario);

            if (EstadoUsuario != null && EstadoUsuarioDto != null)
            {
                response.UpdateData(EstadoUsuarioDto);
            }

            return await Task.FromResult(response);
        }
    }
}
