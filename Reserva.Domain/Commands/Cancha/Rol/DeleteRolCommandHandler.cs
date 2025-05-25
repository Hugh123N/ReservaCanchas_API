using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class DeleteRolCommandHandler : CommandHandlerBase<DeleteRolCommand>
    {
        private readonly IRepository<Entity.Models.Rol> _RolRepository;

        public DeleteRolCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteRolCommandValidator validator,
            IRepository<Entity.Models.Rol> RolRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _RolRepository = RolRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteRolCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Rol = await _RolRepository.GetByAsync(x => x.IdRol == request.Id);

            if (Rol != null)
            {
                Rol.Activo = false;
                await _RolRepository.UpdateAsync(Rol);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
