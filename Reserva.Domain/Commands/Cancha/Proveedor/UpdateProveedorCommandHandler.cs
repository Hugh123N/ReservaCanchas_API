using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class UpdateProveedorCommandHandler : CommandHandlerBase<UpdateProveedorCommand, GetProveedorDto>
    {
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;

        public UpdateProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateProveedorCommandValidator validator,
            IRepository<Entity.Models.Proveedor> ProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ProveedorRepository = ProveedorRepository;
        }

        public override async Task<ResponseDto<GetProveedorDto>> HandleCommand(UpdateProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorDto>();
            var Proveedor = await _ProveedorRepository.GetByAsync(x => x.IdProveedor == request.UpdateDto.IdProveedor);

            if (Proveedor != null)
            {
                _mapper?.Map(request.UpdateDto, Proveedor);
                await _ProveedorRepository.UpdateAsync(Proveedor);
                await _ProveedorRepository.SaveAsync();
            }

            var ProveedorDto = _mapper?.Map<GetProveedorDto>(Proveedor);
            if (ProveedorDto != null) response.UpdateData(ProveedorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
