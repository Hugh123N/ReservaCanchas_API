using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class DeleteIntentoLoginCommandHandler : CommandHandlerBase<DeleteIntentoLoginCommand>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _IntentoLoginRepository;

        public DeleteIntentoLoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteIntentoLoginCommandValidator validator,
            IRepository<Entity.Models.IntentoLogin> IntentoLoginRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _IntentoLoginRepository = IntentoLoginRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteIntentoLoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var IntentoLogin = await _IntentoLoginRepository.GetByAsync(x => x.IdIntentoLogin == request.Id);

            if (IntentoLogin != null)
            {
                IntentoLogin.Activo = false;
                await _IntentoLoginRepository.UpdateAsync(IntentoLogin);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
