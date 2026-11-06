using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class GetUsuarioNegocioQueryHandler : QueryHandlerBase<GetUsuarioNegocioQuery, int?>
    {
        private readonly IRepository<Entity.Cliente> _clienteRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;

        public GetUsuarioNegocioQueryHandler(
            IRepository<Entity.Cliente> clienteRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.Operador> operadorRepository)
        {
            _clienteRepository = clienteRepository;
            _proveedorRepository = proveedorRepository;
            _operadorRepository = operadorRepository;
        }

        protected override async Task<ResponseDto<int?>> HandleQuery(GetUsuarioNegocioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<int?>();
            cancellationToken.ThrowIfCancellationRequested();
            if (request.IdUsuario == Guid.Empty)
            {
                response.AddErrorResult("El usuario de la sesión no es válido.");
                return response;
            }

            var userId = request.IdUsuario.ToString();
            int? idUsuarioNegocio;

            // El contexto público prevalece sobre los roles de gestión de la misma cuenta.
            if (string.Equals(request.ApplicationCode, Constants.Role.Cliente, StringComparison.OrdinalIgnoreCase))
            {
                var cliente = await _clienteRepository.GetByAsNoTrackingAsync(c => c.UserId == userId);
                idUsuarioNegocio = cliente?.IdCliente;
            }
            else if (request.Roles.Any(role => string.Equals(role, Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase)))
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(p => p.IdUsuario == userId && p.Activo);
                idUsuarioNegocio = proveedor?.IdProveedor;
            }
            else if (request.Roles.Any(role => string.Equals(role, Constants.Role.Operador, StringComparison.OrdinalIgnoreCase)))
            {
                var operador = await _operadorRepository.GetByAsNoTrackingAsync(o => o.IdUsuario == userId && o.Activo);
                idUsuarioNegocio = operador?.IdOperador;
            }
            else
            {
                // Los roles sin contexto de cliente/proveedor/operador no requieren este identificador.
                return response;
            }

            if (!idUsuarioNegocio.HasValue || idUsuarioNegocio.Value <= 0)
            {
                response.AddErrorResult("No se encontró el perfil de negocio del usuario para esta aplicación.");
                return response;
            }

            response.UpdateData(idUsuarioNegocio);
            return response;
        }
    }
}
