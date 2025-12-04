using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoSuperficie;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class UpdateTipoSuperficieCommandHandler : CommandHandlerBase<UpdateTipoSuperficieCommand, GetTipoSuperficieDto>
    {
        private readonly IRepository<Entity.TipoSuperficie> _TipoSuperficieRepository;

        public UpdateTipoSuperficieCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateTipoSuperficieCommandValidator validator,
            IRepository<Entity.TipoSuperficie> TipoSuperficieRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoSuperficieRepository = TipoSuperficieRepository;
        }

        public override async Task<ResponseDto<GetTipoSuperficieDto>> HandleCommand(UpdateTipoSuperficieCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoSuperficieDto>();
            var TipoSuperficie = await _TipoSuperficieRepository.GetByAsync(x => x.IdTipoSuperficie == request.UpdateDto.IdTipoSuperficie);

            if (TipoSuperficie != null)
            {
                _mapper?.Map(request.UpdateDto, TipoSuperficie);
                await _TipoSuperficieRepository.UpdateAsync(TipoSuperficie);
                await _TipoSuperficieRepository.SaveAsync();
            }

            var TipoSuperficieDto = _mapper?.Map<GetTipoSuperficieDto>(TipoSuperficie);
            if (TipoSuperficieDto != null) response.UpdateData(TipoSuperficieDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
