using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoUsuario;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class SelectComboEstadoUsuarioQueryHandler : QueryHandlerBase<SelectComboEstadoUsuarioQuery, IEnumerable<SelectComboEstadoUsuarioDto>>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _repository;

        public SelectComboEstadoUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoUsuario> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboEstadoUsuarioDto>>> HandleQuery(SelectComboEstadoUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboEstadoUsuarioDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboEstadoUsuarioDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboEstadoUsuarioDto>());

            return await Task.FromResult(response);
        }
    }
}
