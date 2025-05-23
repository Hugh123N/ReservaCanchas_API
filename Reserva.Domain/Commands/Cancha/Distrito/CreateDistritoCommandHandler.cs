using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class CreateDistritoCommandHandler : CommandHandlerBase<CreateDistritoCommand, GetDistritoDto>
    {
        private readonly IRepository<Entity.Models.Distrito> _DistritoRepository;

        public CreateDistritoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDistritoCommandValidator validator,
            IRepository<Entity.Models.Distrito> DistritoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _DistritoRepository = DistritoRepository;
        }

        public override async Task<ResponseDto<GetDistritoDto>> HandleCommand(CreateDistritoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDistritoDto>();

            var Distrito = _mapper?.Map<Entity.Models.Distrito>(request.CreateDto);

            if (Distrito != null)
            {
                await _DistritoRepository.AddAsync(Distrito);
                await _DistritoRepository.SaveAsync();
            }

            var DistritoDto = _mapper?.Map<GetDistritoDto>(Distrito);
            if (DistritoDto != null) response.UpdateData(DistritoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}