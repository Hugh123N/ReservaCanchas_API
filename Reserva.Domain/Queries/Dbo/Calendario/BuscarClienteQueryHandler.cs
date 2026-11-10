using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.PersonLookup;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class BuscarClienteQueryHandler : QueryHandlerBase<BuscarClienteQuery, ClientDto>
    {
        private readonly IRepository<Entity.Cliente> _clienteRepository;
        private readonly IPersonLookupService _personLookupService;

        public BuscarClienteQueryHandler(
            IRepository<Entity.Cliente> clienteRepository,
            IPersonLookupService personLookupService)
        {
            _clienteRepository = clienteRepository;
            _personLookupService = personLookupService;
        }

        protected override async Task<ResponseDto<ClientDto>> HandleQuery(
            BuscarClienteQuery request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = new ResponseDto<ClientDto>();
            var dni = request.Dni;
            if (string.IsNullOrEmpty(dni) || dni.Length != 8 || dni.Any(character => character < '0' || character > '9'))
            {
                response.AddErrorResult("El DNI debe contener exactamente 8 dígitos numéricos.");
                return response;
            }

            var cliente = await _clienteRepository.GetByAsNoTrackingAsync(
                item => item.Activo && item.TipoDoc == "DNI" && item.NroDoc == dni);
            if (cliente != null)
            {
                response.UpdateData(new ClientDto
                {
                    IdCliente = cliente.IdCliente,
                    EsNuevoCliente = false,
                    NombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}".Trim(),
                    FirstName = cliente.Nombres,
                    LastName = cliente.Apellidos,
                    TipoDoc = cliente.TipoDoc,
                    NroDoc = cliente.NroDoc,
                    Telefono = cliente.Telefono,
                    Email = cliente.Email,
                    Activo = cliente.Activo
                });
                return response;
            }

            var lookup = await _personLookupService.GetDniAsync(dni, cancellationToken);
            if (!lookup.IsValid)
            {
                response.AttachResults(lookup);
                return response;
            }

            if (lookup.Data == null)
            {
                response.AddErrorResult("No se encontraron datos para el DNI indicado.");
                return response;
            }

            var apellidos = string.Join(" ", new[] { lookup.Data.LastName, lookup.Data.MaternalLastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            response.UpdateData(new ClientDto
            {
                EsNuevoCliente = true,
                NombreCompleto = $"{lookup.Data.FirstName} {apellidos}".Trim(),
                FirstName = lookup.Data.FirstName,
                LastName = apellidos,
                TipoDoc = "DNI",
                NroDoc = lookup.Data.DocumentNumber,
                Activo = true
            });
            return response;
        }
    }
}
