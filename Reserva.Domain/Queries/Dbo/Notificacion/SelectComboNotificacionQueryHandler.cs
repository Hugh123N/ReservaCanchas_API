using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Notificacion;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class SelectComboNotificacionQueryHandler : QueryHandlerBase<SelectComboNotificacionQuery, IEnumerable<SelectComboNotificacionDto>>
    {
        private readonly IRepository<Entity.Notificacion> _repository;

        public SelectComboNotificacionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Notificacion> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboNotificacionDto>>> HandleQuery(SelectComboNotificacionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboNotificacionDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboNotificacionDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboNotificacionDto>());

            return await Task.FromResult(response);
        }
    }
}
