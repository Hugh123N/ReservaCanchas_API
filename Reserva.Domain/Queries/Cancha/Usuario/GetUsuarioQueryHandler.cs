using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class GetUsuarioQueryHandler : QueryHandlerBase<GetUsuarioQuery, GetUsuarioDto>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;

        public GetUsuarioQueryHandler(
            IMapper mapper,
            GetUsuarioQueryValidator validator,
            IRepository<Entity.Models.Usuario> UsuarioRepository
        ) : base(mapper, validator)
        {
            _UsuarioRepository = UsuarioRepository;
        }

        protected override async Task<ResponseDto<GetUsuarioDto>> HandleQuery(GetUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();
            var Usuario = await _UsuarioRepository.GetByAsync(x => x.IdUsuario == request.Id);
            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(Usuario);

            if (Usuario != null && UsuarioDto != null)
            {
                response.UpdateData(UsuarioDto);
            }

            return await Task.FromResult(response);
        }
    }
}
