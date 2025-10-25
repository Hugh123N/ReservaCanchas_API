using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class CreateImagenCanchaCommandHandler : CommandHandlerBase<CreateImagenCanchaCommand, GetImagenCanchaDto>
    {
        private readonly IRepository<Entity.ImagenCancha> _ImagenCanchaRepository;

        public CreateImagenCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateImagenCanchaCommandValidator validator,
            IRepository<Entity.ImagenCancha> ImagenCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        public override async Task<ResponseDto<GetImagenCanchaDto>> HandleCommand(CreateImagenCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetImagenCanchaDto>();

            var ImagenCancha = _mapper?.Map<Entity.ImagenCancha>(request.CreateDto);

            if (ImagenCancha != null)
            {
                await _ImagenCanchaRepository.AddAsync(ImagenCancha);
                await _ImagenCanchaRepository.SaveAsync();
            }

            var ImagenCanchaDto = _mapper?.Map<GetImagenCanchaDto>(ImagenCancha);
            if (ImagenCanchaDto != null) response.UpdateData(ImagenCanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}