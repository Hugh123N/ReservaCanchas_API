using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Servicio;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Servicio
{
    public class SelectComboServicioQueryHandler : QueryHandlerBase<SelectComboServicioQuery, IEnumerable<SelectComboServicioDto>>
    {
        private readonly IRepository<Entity.Servicio> _repository;

        public SelectComboServicioQueryHandler(
            IMapper mapper,
            IRepository<Entity.Servicio> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboServicioDto>>> HandleQuery(SelectComboServicioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboServicioDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboServicioDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboServicioDto>());

            return await Task.FromResult(response);
        }
    }
}
