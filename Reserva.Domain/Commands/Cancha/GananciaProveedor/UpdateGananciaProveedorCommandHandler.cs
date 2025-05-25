using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class UpdateGananciaProveedorCommandHandler : CommandHandlerBase<UpdateGananciaProveedorCommand, GetGananciaProveedorDto>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _GananciaProveedorRepository;

        public UpdateGananciaProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateGananciaProveedorCommandValidator validator,
            IRepository<Entity.Models.GananciaProveedor> GananciaProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _GananciaProveedorRepository = GananciaProveedorRepository;
        }

        public override async Task<ResponseDto<GetGananciaProveedorDto>> HandleCommand(UpdateGananciaProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetGananciaProveedorDto>();
            var GananciaProveedor = await _GananciaProveedorRepository.GetByAsync(x => x.IdGananciaProveedor == request.UpdateDto.IdGananciaProveedor);

            if (GananciaProveedor != null)
            {
                _mapper?.Map(request.UpdateDto, GananciaProveedor);
                await _GananciaProveedorRepository.UpdateAsync(GananciaProveedor);
                await _GananciaProveedorRepository.SaveAsync();
            }

            var GananciaProveedorDto = _mapper?.Map<GetGananciaProveedorDto>(GananciaProveedor);
            if (GananciaProveedorDto != null) response.UpdateData(GananciaProveedorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
