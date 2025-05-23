using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class CreateProvinciaCommandHandler : CommandHandlerBase<CreateProvinciaCommand, GetProvinciaDto>
    {
        private readonly IRepository<Entity.Models.Provincia> _ProvinciaRepository;

        public CreateProvinciaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateProvinciaCommandValidator validator,
            IRepository<Entity.Models.Provincia> ProvinciaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ProvinciaRepository = ProvinciaRepository;
        }

        public override async Task<ResponseDto<GetProvinciaDto>> HandleCommand(CreateProvinciaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProvinciaDto>();

            var Provincia = _mapper?.Map<Entity.Models.Provincia>(request.CreateDto);

            if (Provincia != null)
            {
                await _ProvinciaRepository.AddAsync(Provincia);
                await _ProvinciaRepository.SaveAsync();
            }

            var ProvinciaDto = _mapper?.Map<GetProvinciaDto>(Provincia);
            if (ProvinciaDto != null) response.UpdateData(ProvinciaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}