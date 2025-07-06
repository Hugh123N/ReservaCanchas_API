using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class CreateCanchaFavoritaCommandHandler : CommandHandlerBase<CreateCanchaFavoritaCommand, GetCanchaFavoritaDto>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _CanchaFavoritaRepository;

        public CreateCanchaFavoritaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateCanchaFavoritaCommandValidator validator,
            IRepository<Entity.Models.CanchaFavorita> CanchaFavoritaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        public override async Task<ResponseDto<GetCanchaFavoritaDto>> HandleCommand(CreateCanchaFavoritaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaFavoritaDto>();

            var CanchaFavorita = _mapper?.Map<Entity.Models.CanchaFavorita>(request.CreateDto);

            if (CanchaFavorita != null)
            {
                await _CanchaFavoritaRepository.AddAsync(CanchaFavorita);
                await _CanchaFavoritaRepository.SaveAsync();
            }

            var CanchaFavoritaDto = _mapper?.Map<GetCanchaFavoritaDto>(CanchaFavorita);
            if (CanchaFavoritaDto != null) response.UpdateData(CanchaFavoritaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}