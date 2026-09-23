using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cliente;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class UpdateClienteCommandHandler : CommandHandlerBase<UpdateClienteCommand, GetClienteDto>
    {
        private readonly IRepository<Entity.Cliente> _ClienteRepository;

        public UpdateClienteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            UpdateClienteCommandValidator validator,
            IRepository<Entity.Cliente> ClienteRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ClienteRepository = ClienteRepository;
        }

        public override async Task<ResponseDto<GetClienteDto>> HandleCommand(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetClienteDto>();

            var Cliente = await _ClienteRepository.GetByAsync(x => x.IdCliente == request.UpdateDto.IdCliente && x.Activo);

            if (Cliente != null)
            {
                _mapper?.Map(request.UpdateDto, Cliente);
                await _ClienteRepository.UpdateAsync(Cliente);
                await _ClienteRepository.SaveAsync();
            }

            var ClienteDto = _mapper?.Map<GetClienteDto>(Cliente);
            if (ClienteDto != null) response.UpdateData(ClienteDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
