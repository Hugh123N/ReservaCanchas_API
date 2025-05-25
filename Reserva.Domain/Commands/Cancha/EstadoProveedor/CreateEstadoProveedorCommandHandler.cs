using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class CreateEstadoProveedorCommandHandler : CommandHandlerBase<CreateEstadoProveedorCommand, GetEstadoProveedorDto>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;

        public CreateEstadoProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateEstadoProveedorCommandValidator validator,
            IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _EstadoProveedorRepository = EstadoProveedorRepository;
        }

        public override async Task<ResponseDto<GetEstadoProveedorDto>> HandleCommand(CreateEstadoProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoProveedorDto>();

            var EstadoProveedor = _mapper?.Map<Entity.Models.EstadoProveedor>(request.CreateDto);

            if (EstadoProveedor != null)
            {
                await _EstadoProveedorRepository.AddAsync(EstadoProveedor);
                await _EstadoProveedorRepository.SaveAsync();
            }

            var EstadoProveedorDto = _mapper?.Map<GetEstadoProveedorDto>(EstadoProveedor);
            if (EstadoProveedorDto != null) response.UpdateData(EstadoProveedorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}