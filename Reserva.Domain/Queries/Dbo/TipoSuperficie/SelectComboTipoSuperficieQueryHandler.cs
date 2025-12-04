using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoSuperficie;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoSuperficie
{
    public class SelectComboTipoSuperficieQueryHandler : QueryHandlerBase<SelectComboTipoSuperficieQuery, IEnumerable<SelectComboTipoSuperficieDto>>
    {
        private readonly IRepository<Entity.TipoSuperficie> _repository;

        public SelectComboTipoSuperficieQueryHandler(
            IMapper mapper,
            IRepository<Entity.TipoSuperficie> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboTipoSuperficieDto>>> HandleQuery(SelectComboTipoSuperficieQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboTipoSuperficieDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboTipoSuperficieDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboTipoSuperficieDto>());

            return await Task.FromResult(response);
        }
    }
}
