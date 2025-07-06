using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.IntentoLogin;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class CreateIntentoLoginCommandHandler : CommandHandlerBase<CreateIntentoLoginCommand, GetIntentoLoginDto>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _IntentoLoginRepository;

        public CreateIntentoLoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateIntentoLoginCommandValidator validator,
            IRepository<Entity.Models.IntentoLogin> IntentoLoginRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _IntentoLoginRepository = IntentoLoginRepository;
        }

        public override async Task<ResponseDto<GetIntentoLoginDto>> HandleCommand(CreateIntentoLoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetIntentoLoginDto>();

            var IntentoLogin = _mapper?.Map<Entity.Models.IntentoLogin>(request.CreateDto);

            if (IntentoLogin != null)
            {
                await _IntentoLoginRepository.AddAsync(IntentoLogin);
                await _IntentoLoginRepository.SaveAsync();
            }

            var IntentoLoginDto = _mapper?.Map<GetIntentoLoginDto>(IntentoLogin);
            if (IntentoLoginDto != null) response.UpdateData(IntentoLoginDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}