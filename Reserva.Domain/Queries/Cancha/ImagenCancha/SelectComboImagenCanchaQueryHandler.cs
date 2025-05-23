using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class SelectComboImagenCanchaQueryHandler : QueryHandlerBase<SelectComboImagenCanchaQuery, IEnumerable<SelectComboImagenCanchaDto>>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _repository;

        public SelectComboImagenCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.ImagenCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboImagenCanchaDto>>> HandleQuery(SelectComboImagenCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboImagenCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboImagenCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboImagenCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
