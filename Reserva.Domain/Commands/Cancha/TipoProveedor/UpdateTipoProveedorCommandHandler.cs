using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class UpdateTipoProveedorCommandHandler : CommandHandlerBase<UpdateTipoProveedorCommand, GetTipoProveedorDto>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _TipoProveedorRepository;

        public UpdateTipoProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateTipoProveedorCommandValidator validator,
            IRepository<Entity.Models.TipoProveedor> TipoProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        public override async Task<ResponseDto<GetTipoProveedorDto>> HandleCommand(UpdateTipoProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoProveedorDto>();
            var TipoProveedor = await _TipoProveedorRepository.GetByAsync(x => x.IdTipoProveedor == request.UpdateDto.IdTipoProveedor);

            if (TipoProveedor != null)
            {
                _mapper?.Map(request.UpdateDto, TipoProveedor);
                await _TipoProveedorRepository.UpdateAsync(TipoProveedor);
                await _TipoProveedorRepository.SaveAsync();
            }

            var TipoProveedorDto = _mapper?.Map<GetTipoProveedorDto>(TipoProveedor);
            if (TipoProveedorDto != null) response.UpdateData(TipoProveedorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
