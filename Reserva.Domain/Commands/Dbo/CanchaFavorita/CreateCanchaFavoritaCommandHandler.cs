using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.CanchaFavorita
{
    public class CreateCanchaFavoritaCommandHandler : CommandHandlerBase<CreateCanchaFavoritaCommand, GetCanchaFavoritaDto>
    {
        private readonly IRepository<Entity.CanchaFavorita> _CanchaFavoritaRepository;

        public CreateCanchaFavoritaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateCanchaFavoritaCommandValidator validator,
            IRepository<Entity.CanchaFavorita> CanchaFavoritaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        public override async Task<ResponseDto<GetCanchaFavoritaDto>> HandleCommand(CreateCanchaFavoritaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaFavoritaDto>();

            var CanchaFavorita = await _CanchaFavoritaRepository.GetByAsync(
                x => x.IdCancha == request.CreateDto.IdCancha && x.IdUsuario == request.CreateDto.IdUsuario);

            if (CanchaFavorita != null)
            {
                CanchaFavorita.Activo = true;
                await _CanchaFavoritaRepository.UpdateAsync(CanchaFavorita);
                await _CanchaFavoritaRepository.SaveAsync();
            }else{
                CanchaFavorita = _mapper?.Map<Entity.CanchaFavorita>(request.CreateDto);

                if (CanchaFavorita != null)
                {
                    await _CanchaFavoritaRepository.AddAsync(CanchaFavorita);
                    await _CanchaFavoritaRepository.SaveAsync();
                }
            }

            var CanchaFavoritaDto = _mapper?.Map<GetCanchaFavoritaDto>(CanchaFavorita);
            if (CanchaFavoritaDto != null) response.UpdateData(CanchaFavoritaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}