using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoSuperficie;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class CreateTipoSuperficieCommandHandler : CommandHandlerBase<CreateTipoSuperficieCommand, GetTipoSuperficieDto>
    {
        private readonly IRepository<Entity.TipoSuperficie> _TipoSuperficieRepository;

        public CreateTipoSuperficieCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateTipoSuperficieCommandValidator validator,
            IRepository<Entity.TipoSuperficie> TipoSuperficieRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _TipoSuperficieRepository = TipoSuperficieRepository;
        }

        public override async Task<ResponseDto<GetTipoSuperficieDto>> HandleCommand(CreateTipoSuperficieCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoSuperficieDto>();

            var TipoSuperficie = _mapper?.Map<Entity.TipoSuperficie>(request.CreateDto);

            if (TipoSuperficie != null)
            {
                await _TipoSuperficieRepository.AddAsync(TipoSuperficie);
                await _TipoSuperficieRepository.SaveAsync();
            }

            var TipoSuperficieDto = _mapper?.Map<GetTipoSuperficieDto>(TipoSuperficie);
            if (TipoSuperficieDto != null) response.UpdateData(TipoSuperficieDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}