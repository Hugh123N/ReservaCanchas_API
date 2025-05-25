using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Ubigeo;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class CreateUbigeoCommandHandler : CommandHandlerBase<CreateUbigeoCommand, GetUbigeoDto>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _UbigeoRepository;

        public CreateUbigeoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateUbigeoCommandValidator validator,
            IRepository<Entity.Models.Ubigeo> UbigeoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        public override async Task<ResponseDto<GetUbigeoDto>> HandleCommand(CreateUbigeoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUbigeoDto>();

            var Ubigeo = _mapper?.Map<Entity.Models.Ubigeo>(request.CreateDto);

            if (Ubigeo != null)
            {
                await _UbigeoRepository.AddAsync(Ubigeo);
                await _UbigeoRepository.SaveAsync();
            }

            var UbigeoDto = _mapper?.Map<GetUbigeoDto>(Ubigeo);
            if (UbigeoDto != null) response.UpdateData(UbigeoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}