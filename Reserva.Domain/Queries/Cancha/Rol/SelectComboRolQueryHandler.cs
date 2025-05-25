using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class SelectComboRolQueryHandler : QueryHandlerBase<SelectComboRolQuery, IEnumerable<SelectComboRolDto>>
    {
        private readonly IRepository<Entity.Models.Rol> _repository;

        public SelectComboRolQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Rol> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboRolDto>>> HandleQuery(SelectComboRolQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboRolDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboRolDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboRolDto>());

            return await Task.FromResult(response);
        }
    }
}
