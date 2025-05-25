using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoUsuario;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class ListEstadoUsuarioQueryHandler : QueryHandlerBase<ListEstadoUsuarioQuery, IEnumerable<ListEstadoUsuarioDto>>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _repository;

        public ListEstadoUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoUsuario> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListEstadoUsuarioDto>>> HandleQuery(ListEstadoUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListEstadoUsuarioDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdEstadoUsuario == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListEstadoUsuarioDto>>(list);

            response.UpdateData(listDtos ?? new List<ListEstadoUsuarioDto>());

            return await Task.FromResult(response);
        }
    }
}
