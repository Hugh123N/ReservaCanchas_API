using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class ListUsuarioQueryHandler : QueryHandlerBase<ListUsuarioQuery, IEnumerable<ListUsuarioDto>>
    {
        private readonly IRepository<Entity.AspNetUsers> _repository;

        public ListUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.AspNetUsers> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListUsuarioDto>>> HandleQuery(ListUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListUsuarioDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Id == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListUsuarioDto>>(list);

            response.UpdateData(listDtos ?? new List<ListUsuarioDto>());

            return await Task.FromResult(response);
        }
    }
}
