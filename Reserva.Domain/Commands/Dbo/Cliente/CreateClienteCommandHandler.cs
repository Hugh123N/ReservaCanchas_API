using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Domain.Services;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cliente;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class CreateClienteCommandHandler : CommandHandlerBase<CreateClienteCommand, GetClienteDto>
    {
        private readonly IRepository<Entity.Cliente> _ClienteRepository;
        private readonly IPlanLimitValidationService _planLimitService;

        public CreateClienteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateClienteCommandValidator validator,
            IRepository<Entity.Cliente> ClienteRepository,
            IPlanLimitValidationService planLimitService
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ClienteRepository = ClienteRepository;
            _planLimitService = planLimitService;
        }

        public override async Task<ResponseDto<GetClienteDto>> HandleCommand(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetClienteDto>();

            var cliente = _mapper?.Map<Entity.Cliente>(request.CreateDto);

            if(cliente != null) { 
                await _ClienteRepository.AddAsync(cliente);
                await _ClienteRepository.SaveAsync();
            }

            var ClienteDto = _mapper?.Map<GetClienteDto>(cliente);
            if (ClienteDto != null) response.UpdateData(ClienteDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}