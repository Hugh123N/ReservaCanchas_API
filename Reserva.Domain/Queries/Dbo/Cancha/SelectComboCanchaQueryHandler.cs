using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class SelectComboCanchaQueryHandler : QueryHandlerBase<SelectComboCanchaQuery, IEnumerable<SelectComboCanchaDto>>
    {
        private readonly IRepository<Entity.Cancha> _repository;
        private readonly IUserIdentity _userIdentity;

        public SelectComboCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Cancha> repository,
            IUserIdentity userIdentity
        ) : base(mapper)
        {
            _repository = repository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> HandleQuery(SelectComboCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboCanchaDto>>();

            var idProveedor = _userIdentity.GetCurrentUserIdNegocio() ?? 1;

            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo && x.IdProveedor == idProveedor);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
