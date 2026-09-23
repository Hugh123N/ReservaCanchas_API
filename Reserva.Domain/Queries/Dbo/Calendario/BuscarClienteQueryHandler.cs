using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class BuscarClienteQueryHandler : QueryHandlerBase<BuscarClienteQuery, List<ClientDto>>
    {
        private readonly IRepository<Cliente> _clienteRepository;

        public BuscarClienteQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<Cliente> userRepository
        ) : base(mapper, mediator)
        {
            _clienteRepository = userRepository;
        }

        protected override async Task<ResponseDto<List<ClientDto>>> HandleQuery(
            BuscarClienteQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<ClientDto>>();

            var termino = request.TerminoBusqueda.Trim().ToLower();

            if (string.IsNullOrEmpty(termino))
            {
                response.AddErrorResult("El término de búsqueda es requerido");
                return response;
            }

            // Buscar por nombre, apellido o teléfono
            var clientes = await _clienteRepository.FindByAsNoTrackingAsync(
                u => u.Activo &&
                     (u.Nombres.ToLower().Contains(termino) ||
                      u.Apellidos.ToLower().Contains(termino) ||
                      u.Telefono.Contains(termino))
            );

            if (!clientes.Any())
            {
                response.AddErrorResult("No se encontraron clientes con ese criterio de búsqueda");
                return response;
            }

            // Mapear a DTOs
            var clientesDto = clientes.Select(c => new ClientDto
            {
                IdCliente = c.IdCliente,
                NombreCompleto = $"{c.Nombres} {c.Apellidos}".Trim(),
                FirstName = c.Nombres,
                LastName = c.Apellidos,
                Telefono = c.Telefono,
                Email = c.Email,
                Activo = c.Activo
            }).ToList();

            response.UpdateData(clientesDto);
            return response;
        }
    }
}
