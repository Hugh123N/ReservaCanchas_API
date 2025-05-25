using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class SelectComboReservaQueryHandler : QueryHandlerBase<SelectComboReservaQuery, IEnumerable<SelectComboReservaDto>>
    {
        private readonly IRepository<Entity.Models.Reserva> _repository;

        public SelectComboReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Reserva> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> HandleQuery(SelectComboReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboReservaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboReservaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboReservaDto>());

            return await Task.FromResult(response);
        }
    }
}
