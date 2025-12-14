using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Operador;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class GetOperadorQueryHandler : QueryHandlerBase<GetOperadorQuery, GetOperadorDto>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;

        public GetOperadorQueryHandler(
            IMapper mapper,
            GetOperadorQueryValidator validator,
            IRepository<Entity.Operador> OperadorRepository
        ) : base(mapper, validator)
        {
            _OperadorRepository = OperadorRepository;
        }

        protected override async Task<ResponseDto<GetOperadorDto>> HandleQuery(GetOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetOperadorDto>();
            var Operador = await _OperadorRepository.GetByAsync(x => x.IdOperador == request.Id, 
                x => x.IdUsuarioNavigation,
                x => x.OperadorCancha.Where(x => x.Activo));

            var OperadorDto = new GetOperadorDto
            {
                IdUsuario = Operador.IdUsuario,
                IdProveedor = Operador.IdProveedor,
                Nombre = Operador?.IdUsuarioNavigation?.FirstName ?? string.Empty,
                Apellidos = Operador?.IdUsuarioNavigation?.LastName ?? string.Empty,
                Email = Operador?.IdUsuarioNavigation?.Email ?? string.Empty,
                Telefono = Operador?.IdUsuarioNavigation?.PhoneNumber ?? string.Empty,
                Imagen = Operador?.IdUsuarioNavigation?.Imagen ?? string.Empty, 
                FechaCreacion = Operador.CreateDate,
                FechaActualizacion = Operador.UpdateDate,
                CanchaIds = Operador.OperadorCancha.Select(oc => oc.IdCancha).ToList()
            };

            response.UpdateData(OperadorDto);

            return await Task.FromResult(response);
        }
    }
}
