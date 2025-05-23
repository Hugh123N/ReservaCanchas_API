using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class UpdateTipoCanchaCommandHandler : CommandHandlerBase<UpdateTipoCanchaCommand, GetTipoCanchaDto>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _TipoCanchaRepository;

        public UpdateTipoCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateTipoCanchaCommandValidator validator,
            IRepository<Entity.Models.TipoCancha> TipoCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoCanchaRepository = TipoCanchaRepository;
        }

        public override async Task<ResponseDto<GetTipoCanchaDto>> HandleCommand(UpdateTipoCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoCanchaDto>();
            var TipoCancha = await _TipoCanchaRepository.GetByAsync(x => x.IdTipoCancha == request.UpdateDto.IdTipoCancha);

            if (TipoCancha != null)
            {
                _mapper?.Map(request.UpdateDto, TipoCancha);
                await _TipoCanchaRepository.UpdateAsync(TipoCancha);
                await _TipoCanchaRepository.SaveAsync();
            }

            var TipoCanchaDto = _mapper?.Map<GetTipoCanchaDto>(TipoCancha);
            if (TipoCanchaDto != null) response.UpdateData(TipoCanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
