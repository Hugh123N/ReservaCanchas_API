using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoDeporte
{
    public class SelectComboTipoDeporteQueryHandler : QueryHandlerBase<SelectComboTipoDeporteQuery, IEnumerable<SelectComboTipoDeporteDto>>
    {
        private readonly IRepository<Entity.TipoDeporte> _repository;

        public SelectComboTipoDeporteQueryHandler(
            IMapper mapper,
            IRepository<Entity.TipoDeporte> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboTipoDeporteDto>>> HandleQuery(SelectComboTipoDeporteQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboTipoDeporteDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboTipoDeporteDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboTipoDeporteDto>());

            return await Task.FromResult(response);
        }
    }
}
