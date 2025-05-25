using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class SelectComboUsuarioQueryHandler : QueryHandlerBase<SelectComboUsuarioQuery, IEnumerable<SelectComboUsuarioDto>>
    {
        private readonly IRepository<Entity.Models.Usuario> _repository;

        public SelectComboUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Usuario> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboUsuarioDto>>> HandleQuery(SelectComboUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboUsuarioDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboUsuarioDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboUsuarioDto>());

            return await Task.FromResult(response);
        }
    }
}
