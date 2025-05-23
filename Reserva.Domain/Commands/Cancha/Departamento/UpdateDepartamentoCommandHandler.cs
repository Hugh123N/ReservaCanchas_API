using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class UpdateDepartamentoCommandHandler : CommandHandlerBase<UpdateDepartamentoCommand, GetDepartamentoDto>
    {
        private readonly IRepository<Entity.Models.Departamento> _DepartamentoRepository;

        public UpdateDepartamentoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDepartamentoCommandValidator validator,
            IRepository<Entity.Models.Departamento> DepartamentoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DepartamentoRepository = DepartamentoRepository;
        }

        public override async Task<ResponseDto<GetDepartamentoDto>> HandleCommand(UpdateDepartamentoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDepartamentoDto>();
            var Departamento = await _DepartamentoRepository.GetByAsync(x => x.IdDepartamento == request.UpdateDto.IdDepartamento);

            if (Departamento != null)
            {
                _mapper?.Map(request.UpdateDto, Departamento);
                await _DepartamentoRepository.UpdateAsync(Departamento);
                await _DepartamentoRepository.SaveAsync();
            }

            var DepartamentoDto = _mapper?.Map<GetDepartamentoDto>(Departamento);
            if (DepartamentoDto != null) response.UpdateData(DepartamentoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
