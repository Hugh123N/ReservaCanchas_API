using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class UpdateEstadoCanchaCommandHandler : CommandHandlerBase<UpdateEstadoCanchaCommand, GetEstadoCanchaDto>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public UpdateEstadoCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateEstadoCanchaCommandValidator validator,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        public override async Task<ResponseDto<GetEstadoCanchaDto>> HandleCommand(UpdateEstadoCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoCanchaDto>();
            var EstadoCancha = await _EstadoCanchaRepository.GetByAsync(x => x.IdEstadoCancha == request.UpdateDto.IdEstadoCancha);

            if (EstadoCancha != null)
            {
                _mapper?.Map(request.UpdateDto, EstadoCancha);
                await _EstadoCanchaRepository.UpdateAsync(EstadoCancha);
                await _EstadoCanchaRepository.SaveAsync();
            }

            var EstadoCanchaDto = _mapper?.Map<GetEstadoCanchaDto>(EstadoCancha);
            if (EstadoCanchaDto != null) response.UpdateData(EstadoCanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
