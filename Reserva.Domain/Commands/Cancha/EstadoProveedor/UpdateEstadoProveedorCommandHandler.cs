using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class UpdateEstadoProveedorCommandHandler : CommandHandlerBase<UpdateEstadoProveedorCommand, GetEstadoProveedorDto>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;

        public UpdateEstadoProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateEstadoProveedorCommandValidator validator,
            IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoProveedorRepository = EstadoProveedorRepository;
        }

        public override async Task<ResponseDto<GetEstadoProveedorDto>> HandleCommand(UpdateEstadoProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoProveedorDto>();
            var EstadoProveedor = await _EstadoProveedorRepository.GetByAsync(x => x.IdEstadoProveedor == request.UpdateDto.IdEstadoProveedor);

            if (EstadoProveedor != null)
            {
                _mapper?.Map(request.UpdateDto, EstadoProveedor);
                await _EstadoProveedorRepository.UpdateAsync(EstadoProveedor);
                await _EstadoProveedorRepository.SaveAsync();
            }

            var EstadoProveedorDto = _mapper?.Map<GetEstadoProveedorDto>(EstadoProveedor);
            if (EstadoProveedorDto != null) response.UpdateData(EstadoProveedorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
