using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class UpdateCanchaFavoritaCommandHandler : CommandHandlerBase<UpdateCanchaFavoritaCommand, GetCanchaFavoritaDto>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _CanchaFavoritaRepository;

        public UpdateCanchaFavoritaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateCanchaFavoritaCommandValidator validator,
            IRepository<Entity.Models.CanchaFavorita> CanchaFavoritaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        public override async Task<ResponseDto<GetCanchaFavoritaDto>> HandleCommand(UpdateCanchaFavoritaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaFavoritaDto>();
            var CanchaFavorita = await _CanchaFavoritaRepository.GetByAsync(x => x.IdCancha == request.UpdateDto.IdCanchaFavorita);

            if (CanchaFavorita != null)
            {
                _mapper?.Map(request.UpdateDto, CanchaFavorita);
                await _CanchaFavoritaRepository.UpdateAsync(CanchaFavorita);
                await _CanchaFavoritaRepository.SaveAsync();
            }

            var CanchaFavoritaDto = _mapper?.Map<GetCanchaFavoritaDto>(CanchaFavorita);
            if (CanchaFavoritaDto != null) response.UpdateData(CanchaFavoritaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
