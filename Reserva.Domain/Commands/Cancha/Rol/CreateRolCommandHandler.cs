using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class CreateRolCommandHandler : CommandHandlerBase<CreateRolCommand, GetRolDto>
    {
        private readonly IRepository<Entity.Models.Rol> _RolRepository;

        public CreateRolCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateRolCommandValidator validator,
            IRepository<Entity.Models.Rol> RolRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _RolRepository = RolRepository;
        }

        public override async Task<ResponseDto<GetRolDto>> HandleCommand(CreateRolCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetRolDto>();

            var Rol = _mapper?.Map<Entity.Models.Rol>(request.CreateDto);

            if (Rol != null)
            {
                await _RolRepository.AddAsync(Rol);
                await _RolRepository.SaveAsync();
            }

            var RolDto = _mapper?.Map<GetRolDto>(Rol);
            if (RolDto != null) response.UpdateData(RolDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}