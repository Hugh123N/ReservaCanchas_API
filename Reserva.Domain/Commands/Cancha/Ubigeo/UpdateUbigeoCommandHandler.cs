using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Ubigeo;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class UpdateUbigeoCommandHandler : CommandHandlerBase<UpdateUbigeoCommand, GetUbigeoDto>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _UbigeoRepository;

        public UpdateUbigeoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateUbigeoCommandValidator validator,
            IRepository<Entity.Models.Ubigeo> UbigeoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        public override async Task<ResponseDto<GetUbigeoDto>> HandleCommand(UpdateUbigeoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUbigeoDto>();
            var Ubigeo = await _UbigeoRepository.GetByAsync(x => x.CodigoUbigeo == request.UpdateDto.CodigoUbigeo);

            if (Ubigeo != null)
            {
                _mapper?.Map(request.UpdateDto, Ubigeo);
                await _UbigeoRepository.UpdateAsync(Ubigeo);
                await _UbigeoRepository.SaveAsync();
            }

            var UbigeoDto = _mapper?.Map<GetUbigeoDto>(Ubigeo);
            if (UbigeoDto != null) response.UpdateData(UbigeoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
