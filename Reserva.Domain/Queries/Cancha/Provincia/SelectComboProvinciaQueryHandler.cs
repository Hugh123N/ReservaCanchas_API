using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class SelectComboProvinciaQueryHandler : QueryHandlerBase<SelectComboProvinciaQuery, IEnumerable<SelectComboProvinciaDto>>
    {
        private readonly IRepository<Entity.Models.Provincia> _repository;

        public SelectComboProvinciaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Provincia> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboProvinciaDto>>> HandleQuery(SelectComboProvinciaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboProvinciaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboProvinciaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboProvinciaDto>());

            return await Task.FromResult(response);
        }
    }
}
