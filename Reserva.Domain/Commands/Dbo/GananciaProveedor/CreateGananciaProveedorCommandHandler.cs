using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.GananciaProveedor;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.GananciaProveedor
{
    public class CreateGananciaProveedorCommandHandler : CommandHandlerBase<CreateGananciaProveedorCommand, GetGananciaProveedorDto>
    {
        private readonly IRepository<Entity.GananciaProveedor> _GananciaProveedorRepository;

        public CreateGananciaProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateGananciaProveedorCommandValidator validator,
            IRepository<Entity.GananciaProveedor> GananciaProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _GananciaProveedorRepository = GananciaProveedorRepository;
        }

        public override async Task<ResponseDto<GetGananciaProveedorDto>> HandleCommand(CreateGananciaProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetGananciaProveedorDto>();

            var GananciaProveedor = _mapper?.Map<Entity.GananciaProveedor>(request.CreateDto);

            if (GananciaProveedor != null)
            {
                await _GananciaProveedorRepository.AddAsync(GananciaProveedor);
                await _GananciaProveedorRepository.SaveAsync();
            }

            var GananciaProveedorDto = _mapper?.Map<GetGananciaProveedorDto>(GananciaProveedor);
            if (GananciaProveedorDto != null) response.UpdateData(GananciaProveedorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}