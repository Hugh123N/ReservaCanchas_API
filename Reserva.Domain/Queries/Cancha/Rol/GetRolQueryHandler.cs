using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class GetRolQueryHandler : QueryHandlerBase<GetRolQuery, GetRolDto>
    {
        private readonly IRepository<Entity.Models.Rol> _RolRepository;

        public GetRolQueryHandler(
            IMapper mapper,
            GetRolQueryValidator validator,
            IRepository<Entity.Models.Rol> RolRepository
        ) : base(mapper, validator)
        {
            _RolRepository = RolRepository;
        }

        protected override async Task<ResponseDto<GetRolDto>> HandleQuery(GetRolQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetRolDto>();
            var Rol = await _RolRepository.GetByAsync(x => x.IdRol == request.Id);
            var RolDto = _mapper?.Map<GetRolDto>(Rol);

            if (Rol != null && RolDto != null)
            {
                response.UpdateData(RolDto);
            }

            return await Task.FromResult(response);
        }
    }
}
