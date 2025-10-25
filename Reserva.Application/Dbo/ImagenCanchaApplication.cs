using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.ImagenCancha;
using Reserva.Domain.Queries.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Application.Dbo
{
    public class ImagenCanchaApplication : ApplicationBase, IImagenCanchaApplication
    {
        public ImagenCanchaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetImagenCanchaDto>> Create(CreateImagenCanchaDto createDto)
            => await _mediator.Send(new CreateImagenCanchaCommand(createDto));
        public async Task<ResponseDto<GetImagenCanchaDto>> Update(UpdateImagenCanchaDto updateDto)
            => await _mediator.Send(new UpdateImagenCanchaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteImagenCanchaCommand(id));
        public async Task<ResponseDto<GetImagenCanchaDto>> Get(int id)
            => await _mediator.Send(new GetImagenCanchaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListImagenCanchaDto>>> List(int id)
            => await _mediator.Send(new ListImagenCanchaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchImagenCanchaDto>>> Search(SearchParamsDto<SearchImagenCanchaFilterDto> searchParams)
            => await _mediator.Send(new SearchImagenCanchaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboImagenCanchaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboImagenCanchaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectImagenCanchaDto>>> Select(SearchParamsDto<SelectImagenCanchaFilterDto> searchParams)
             => await _mediator.Send(new SelectImagenCanchaQuery(searchParams));

    }
}
