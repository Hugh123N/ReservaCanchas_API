using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class BuscarClienteQueryHandler : QueryHandlerBase<BuscarClienteQuery, List<ClienteDto>>
    {
        private readonly IRepository<AspNetUsers> _userRepository;

        public BuscarClienteQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<AspNetUsers> userRepository
        ) : base(mapper, mediator)
        {
            _userRepository = userRepository;
        }

        protected override async Task<ResponseDto<List<ClienteDto>>> HandleQuery(
            BuscarClienteQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<ClienteDto>>();

            var termino = request.TerminoBusqueda.Trim().ToLower();

            if (string.IsNullOrEmpty(termino))
            {
                response.AddErrorResult("El término de búsqueda es requerido");
                return response;
            }

            // Buscar por nombre, apellido o teléfono
            var clientes = await _userRepository.FindByAsync(
                u => u.Activo &&
                     (u.FirstName.ToLower().Contains(termino) ||
                      u.LastName.ToLower().Contains(termino) ||
                      u.PhoneNumber.Contains(termino))
            );

            if (!clientes.Any())
            {
                response.AddErrorResult("No se encontraron clientes con ese criterio de búsqueda");
                return response;
            }

            // Mapear a DTOs
            var clientesDto = clientes.Select(c => new ClienteDto
            {
                IdCliente = c.Id,
                NombreCompleto = $"{c.FirstName} {c.LastName}".Trim(),
                FirstName = c.FirstName,
                LastName = c.LastName,
                Telefono = c.PhoneNumber,
                Email = c.Email,
                Activo = c.Activo
            }).ToList();

            response.UpdateData(clientesDto);
            return response;
        }
    }
}
