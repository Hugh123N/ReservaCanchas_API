using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class ListRolQueryHandler : QueryHandlerBase<ListRolQuery, IEnumerable<ListRolDto>>
    {
        private readonly IRepository<Entity.Models.Rol> _repository;

        public ListRolQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Rol> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListRolDto>>> HandleQuery(ListRolQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListRolDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdRol == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListRolDto>>(list);

            response.UpdateData(listDtos ?? new List<ListRolDto>());

            return await Task.FromResult(response);
        }
    }
}
