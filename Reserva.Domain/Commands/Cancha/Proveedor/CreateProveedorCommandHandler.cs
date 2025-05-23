using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class CreateProveedorCommandHandler : CommandHandlerBase<CreateProveedorCommand, GetProveedorDto>
    {
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;

        public CreateProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateProveedorCommandValidator validator,
            IRepository<Entity.Models.Proveedor> ProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ProveedorRepository = ProveedorRepository;
        }

        public override async Task<ResponseDto<GetProveedorDto>> HandleCommand(CreateProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorDto>();

            var Proveedor = _mapper?.Map<Entity.Models.Proveedor>(request.CreateDto);

            if (Proveedor != null)
            {
                await _ProveedorRepository.AddAsync(Proveedor);
                await _ProveedorRepository.SaveAsync();
            }

            var ProveedorDto = _mapper?.Map<GetProveedorDto>(Proveedor);
            if (ProveedorDto != null) response.UpdateData(ProveedorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}