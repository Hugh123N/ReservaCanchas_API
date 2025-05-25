using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class SelectComboCanchaQueryHandler : QueryHandlerBase<SelectComboCanchaQuery, IEnumerable<SelectComboCanchaDto>>
    {
        private readonly IRepository<Entity.Models.Cancha> _repository;

        public SelectComboCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Cancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> HandleQuery(SelectComboCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
