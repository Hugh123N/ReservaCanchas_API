using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.CanchaFavorita;
using Reserva.Domain.Queries.Cancha.CanchaFavorita;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Application.Cancha
{
    public class CanchaFavoritaApplication : ApplicationBase, ICanchaFavoritaApplication
    {
        public CanchaFavoritaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetCanchaFavoritaDto>> Create(CreateCanchaFavoritaDto createDto)
            => await _mediator.Send(new CreateCanchaFavoritaCommand(createDto));
        public async Task<ResponseDto<GetCanchaFavoritaDto>> Update(UpdateCanchaFavoritaDto updateDto)
            => await _mediator.Send(new UpdateCanchaFavoritaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteCanchaFavoritaCommand(id));
        public async Task<ResponseDto<GetCanchaFavoritaDto>> Get(int id)
            => await _mediator.Send(new GetCanchaFavoritaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListCanchaFavoritaDto>>> List(int id)
            => await _mediator.Send(new ListCanchaFavoritaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchCanchaFavoritaDto>>> Search(SearchParamsDto<SearchCanchaFavoritaFilterDto> searchParams)
            => await _mediator.Send(new SearchCanchaFavoritaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaFavoritaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboCanchaFavoritaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectCanchaFavoritaDto>>> Select(SearchParamsDto<SelectCanchaFavoritaFilterDto> searchParams)
             => await _mediator.Send(new SelectCanchaFavoritaQuery(searchParams));

    }
}
