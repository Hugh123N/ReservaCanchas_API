using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Proveedor
{
    public class GetProveedorQueryHandler : QueryHandlerBase<GetProveedorQuery, GetProveedorDto>
    {
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;
        private readonly UserManager<Entity.ApplicationUser> _userManager;

        public GetProveedorQueryHandler(
            IMapper mapper,
            GetProveedorQueryValidator validator,
            IRepository<Entity.Proveedor> ProveedorRepository,
            UserManager<Entity.ApplicationUser> userManager
        ) : base(mapper, validator)
        {
            _ProveedorRepository = ProveedorRepository;
            _userManager = userManager;
        }

        protected override async Task<ResponseDto<GetProveedorDto>> HandleQuery(GetProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorDto>();
            var Proveedor = await _ProveedorRepository.GetByAsync(x => x.IdProveedor == request.Id);
            var ProveedorDto = _mapper?.Map<GetProveedorDto>(Proveedor);

            if (Proveedor != null && ProveedorDto != null && !string.IsNullOrEmpty(Proveedor.IdUsuario) && Guid.TryParse(Proveedor.IdUsuario, out var idUsuario))
            {
                var user = await _userManager.FindByIdAsync(idUsuario.ToString());
                ProveedorDto.UserName = user?.UserName;
                ProveedorDto.Email = user?.Email;
                response.UpdateData(ProveedorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
