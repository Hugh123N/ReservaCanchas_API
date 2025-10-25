using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoCancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoCancha
{
    public class CreateTipoCanchaCommandHandler : CommandHandlerBase<CreateTipoCanchaCommand, GetTipoCanchaDto>
    {
        private readonly IRepository<Entity.TipoCancha> _TipoCanchaRepository;

        public CreateTipoCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateTipoCanchaCommandValidator validator,
            IRepository<Entity.TipoCancha> TipoCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _TipoCanchaRepository = TipoCanchaRepository;
        }

        public override async Task<ResponseDto<GetTipoCanchaDto>> HandleCommand(CreateTipoCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoCanchaDto>();

            var TipoCancha = _mapper?.Map<Entity.TipoCancha>(request.CreateDto);

            if (TipoCancha != null)
            {
                await _TipoCanchaRepository.AddAsync(TipoCancha);
                await _TipoCanchaRepository.SaveAsync();
            }

            var TipoCanchaDto = _mapper?.Map<GetTipoCanchaDto>(TipoCancha);
            if (TipoCanchaDto != null) response.UpdateData(TipoCanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}