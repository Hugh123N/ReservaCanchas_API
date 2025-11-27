using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.CanchaFavorita;
using Reserva.Domain.Queries.Dbo.CanchaFavorita;
using Reserva.Dto.Dbo.CanchaFavorita;

namespace Reserva.Application.Dbo
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
        public async Task<ResponseDto> Delete(int id, string idUsuario)
            => await _mediator.Send(new DeleteCanchaFavoritaCommand(id, idUsuario));
        public async Task<ResponseDto<GetCanchaFavoritaDto>> Get(int id)
            => await _mediator.Send(new GetCanchaFavoritaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListCanchaFavoritaDto>>> List(string idUsuario)
            => await _mediator.Send(new ListCanchaFavoritaQuery(idUsuario));
        public async Task<ResponseDto<SearchResultDto<SearchCanchaFavoritaDto>>> Search(SearchParamsDto<SearchCanchaFavoritaFilterDto> searchParams)
            => await _mediator.Send(new SearchCanchaFavoritaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaFavoritaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboCanchaFavoritaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectCanchaFavoritaDto>>> Select(SearchParamsDto<SelectCanchaFavoritaFilterDto> searchParams)
             => await _mediator.Send(new SelectCanchaFavoritaQuery(searchParams));

    }
}
