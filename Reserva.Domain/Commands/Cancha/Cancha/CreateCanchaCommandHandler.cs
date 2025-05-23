using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class CreateCanchaCommandHandler : CommandHandlerBase<CreateCanchaCommand, GetCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public CreateCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateCanchaCommandValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _CanchaRepository = CanchaRepository;
        }

        public override async Task<ResponseDto<GetCanchaDto>> HandleCommand(CreateCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();

            var Cancha = _mapper?.Map<Entity.Models.Cancha>(request.CreateDto);

            if (Cancha != null)
            {
                await _CanchaRepository.AddAsync(Cancha);
                await _CanchaRepository.SaveAsync();
            }

            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            if (CanchaDto != null) response.UpdateData(CanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}