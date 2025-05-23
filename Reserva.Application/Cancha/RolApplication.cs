using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Rol;
using Reserva.Domain.Queries.Cancha.Rol;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Application.Cancha
{
    public class RolApplication : ApplicationBase, IRolApplication
    {
        public RolApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetRolDto>> Create(CreateRolDto createDto)
            => await _mediator.Send(new CreateRolCommand(createDto));
        public async Task<ResponseDto<GetRolDto>> Update(UpdateRolDto updateDto)
            => await _mediator.Send(new UpdateRolCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteRolCommand(id));
        public async Task<ResponseDto<GetRolDto>> Get(int id)
            => await _mediator.Send(new GetRolQuery(id));
        public async Task<ResponseDto<IEnumerable<ListRolDto>>> List(int id)
            => await _mediator.Send(new ListRolQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchRolDto>>> Search(SearchParamsDto<SearchRolFilterDto> searchParams)
            => await _mediator.Send(new SearchRolQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboRolDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboRolQuery());
        public async Task<ResponseDto<SearchResultDto<SelectRolDto>>> Select(SearchParamsDto<SelectRolFilterDto> searchParams)
             => await _mediator.Send(new SelectRolQuery(searchParams));

    }
}
