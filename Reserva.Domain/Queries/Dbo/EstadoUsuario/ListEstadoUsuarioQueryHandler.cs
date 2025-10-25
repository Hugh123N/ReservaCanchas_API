using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoUsuario;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoUsuario
{
    public class ListEstadoUsuarioQueryHandler : QueryHandlerBase<ListEstadoUsuarioQuery, IEnumerable<ListEstadoUsuarioDto>>
    {
        private readonly IRepository<Entity.EstadoUsuario> _repository;

        public ListEstadoUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.EstadoUsuario> repository
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
