using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class CreateDepartamentoCommandHandler : CommandHandlerBase<CreateDepartamentoCommand, GetDepartamentoDto>
    {
        private readonly IRepository<Entity.Models.Departamento> _DepartamentoRepository;

        public CreateDepartamentoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDepartamentoCommandValidator validator,
            IRepository<Entity.Models.Departamento> DepartamentoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _DepartamentoRepository = DepartamentoRepository;
        }

        public override async Task<ResponseDto<GetDepartamentoDto>> HandleCommand(CreateDepartamentoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDepartamentoDto>();

            var Departamento = _mapper?.Map<Entity.Models.Departamento>(request.CreateDto);

            if (Departamento != null)
            {
                await _DepartamentoRepository.AddAsync(Departamento);
                await _DepartamentoRepository.SaveAsync();
            }

            var DepartamentoDto = _mapper?.Map<GetDepartamentoDto>(Departamento);
            if (DepartamentoDto != null) response.UpdateData(DepartamentoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}