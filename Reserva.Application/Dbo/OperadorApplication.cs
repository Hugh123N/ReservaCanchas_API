using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Operador;
using Reserva.Domain.Queries.Dbo.Operador;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Application.Dbo
{
    public class OperadorApplication : ApplicationBase, IOperadorApplication
    {
        public OperadorApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetOperadorDto>> Create(CreateOperadorDto createDto)
            => await _mediator.Send(new CreateOperadorCommand(createDto));
        public async Task<ResponseDto<GetOperadorDto>> Update(UpdateOperadorDto updateDto)
            => await _mediator.Send(new UpdateOperadorCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteOperadorCommand(id));
        public async Task<ResponseDto<GetOperadorDto>> Get(int id)
            => await _mediator.Send(new GetOperadorQuery(id));
        public async Task<ResponseDto<IEnumerable<ListOperadorDto>>> List(int id)
            => await _mediator.Send(new ListOperadorQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchOperadorDto>>> Search(SearchParamsDto<SearchOperadorFilterDto> searchParams)
            => await _mediator.Send(new SearchOperadorQuery(searchParams));

    }
}
