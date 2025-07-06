using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.IntentoLogin;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class UpdateIntentoLoginCommandHandler : CommandHandlerBase<UpdateIntentoLoginCommand, GetIntentoLoginDto>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _IntentoLoginRepository;

        public UpdateIntentoLoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateIntentoLoginCommandValidator validator,
            IRepository<Entity.Models.IntentoLogin> IntentoLoginRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _IntentoLoginRepository = IntentoLoginRepository;
        }

        public override async Task<ResponseDto<GetIntentoLoginDto>> HandleCommand(UpdateIntentoLoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetIntentoLoginDto>();
            var IntentoLogin = await _IntentoLoginRepository.GetByAsync(x => x.IdIntentoLogin == request.UpdateDto.IdIntentoLogin);

            if (IntentoLogin != null)
            {
                _mapper?.Map(request.UpdateDto, IntentoLogin);
                await _IntentoLoginRepository.UpdateAsync(IntentoLogin);
                await _IntentoLoginRepository.SaveAsync();
            }

            var IntentoLoginDto = _mapper?.Map<GetIntentoLoginDto>(IntentoLogin);
            if (IntentoLoginDto != null) response.UpdateData(IntentoLoginDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
