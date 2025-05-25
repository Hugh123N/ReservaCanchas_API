using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class SelectComboTipoCanchaQueryHandler : QueryHandlerBase<SelectComboTipoCanchaQuery, IEnumerable<SelectComboTipoCanchaDto>>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _repository;

        public SelectComboTipoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.TipoCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboTipoCanchaDto>>> HandleQuery(SelectComboTipoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboTipoCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboTipoCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboTipoCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
