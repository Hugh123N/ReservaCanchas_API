using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoProveedor;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class CreateTipoProveedorCommandHandler : CommandHandlerBase<CreateTipoProveedorCommand, GetTipoProveedorDto>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _TipoProveedorRepository;

        public CreateTipoProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateTipoProveedorCommandValidator validator,
            IRepository<Entity.Models.TipoProveedor> TipoProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        public override async Task<ResponseDto<GetTipoProveedorDto>> HandleCommand(CreateTipoProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoProveedorDto>();

            var TipoProveedor = _mapper?.Map<Entity.Models.TipoProveedor>(request.CreateDto);

            if (TipoProveedor != null)
            {
                await _TipoProveedorRepository.AddAsync(TipoProveedor);
                await _TipoProveedorRepository.SaveAsync();
            }

            var TipoProveedorDto = _mapper?.Map<GetTipoProveedorDto>(TipoProveedor);
            if (TipoProveedorDto != null) response.UpdateData(TipoProveedorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}