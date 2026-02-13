using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class GetCanchasUsuarioQueryHandler : QueryHandlerBase<GetCanchasUsuarioQuery, List<CanchaUsuarioDto>>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.OperadorCancha> _operadorCanchaRepository;
        private readonly IRepository<Entity.TipoDeporteCancha> _tipoDeporteCanchaRepository;
        private readonly IRepository<Entity.TipoDeporte> _tipoDeporteRepository;
        private readonly IUserIdentity _userIdentity;

        public GetCanchasUsuarioQueryHandler(
            IMapper mapper,
            IMediator mediator,
            GetCanchasUsuarioQueryValidator validator,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.OperadorCancha> operadorCanchaRepository,
            IRepository<Entity.TipoDeporteCancha> tipoDeporteCanchaRepository,
            IRepository<Entity.TipoDeporte> tipoDeporteRepository,
            IUserIdentity userIdentity
        ) : base(mapper, mediator, validator)
        {
            _canchaRepository = canchaRepository;
            _proveedorRepository = proveedorRepository;
            _operadorCanchaRepository = operadorCanchaRepository;
            _tipoDeporteCanchaRepository = tipoDeporteCanchaRepository;
            _tipoDeporteRepository = tipoDeporteRepository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<List<CanchaUsuarioDto>>> HandleQuery(GetCanchasUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<CanchaUsuarioDto>>();
            var canchasDto = new List<CanchaUsuarioDto>();

            try
            {
                List<Entity.Cancha> canchas;

                var userIdNegocio = _userIdentity.GetCurrentUserIdNegocio();

                if (request.Roles.Any(r => string.Equals(r.Trim(),Constants.Role.Proveedor.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    canchas = (await _canchaRepository.FindByAsync(c => c.IdProveedor == userIdNegocio && c.Activo)).ToList();
                }
                else if (request.Roles.Any(r => string.Equals(r.Trim(), Constants.Role.Operador.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    var operadorCanchas = await _operadorCanchaRepository
                        .FindAll()
                        .Where(oc => oc.IdOperador == userIdNegocio && oc.Activo).Select(oc => oc.IdCancha).ToListAsync(cancellationToken);

                    if (!operadorCanchas.Any())
                    {
                        response.UpdateData(canchasDto);
                        return response;
                    }

                    canchas = (await _canchaRepository.FindByAsync(c => operadorCanchas.Contains(c.IdCancha) && c.Activo)).ToList();
                }
                else
                {
                    response.AddErrorResult("El usuario no tiene un rol válido para acceder a las canchas");
                    return response;
                }

                foreach (var cancha in canchas.OrderBy(c => c.Nombre))
                {
                    var tipoDeporteCancha = await _tipoDeporteCanchaRepository
                        .FindAll()
                        .Where(tdc => tdc.IdCancha == cancha.IdCancha && tdc.Activo)
                        .FirstOrDefaultAsync(cancellationToken);

                    string nombreTipoDeporte = string.Empty;
                    int idTipoDeporte = 0;

                    if (tipoDeporteCancha != null)
                    {
                        var tipoDeporte = await _tipoDeporteRepository
                            .GetByAsync(td => td.IdTipoDeporte == tipoDeporteCancha.IdTipoDeporte);

                        if (tipoDeporte != null)
                        {
                            nombreTipoDeporte = tipoDeporte.Nombre;
                            idTipoDeporte = tipoDeporte.IdTipoDeporte;
                        }
                    }

                    canchasDto.Add(new CanchaUsuarioDto
                    {
                        IdCancha = cancha.IdCancha,
                        Nombre = cancha.Nombre,
                        IdProveedor = cancha.IdProveedor,
                        IdTipoCancha = idTipoDeporte,
                        TipoCancha = nombreTipoDeporte
                    });
                }

                response.UpdateData(canchasDto);
            }
            catch (Exception ex)
            {
                response.AddErrorResult("Error al obtener las canchas del usuario", ex);
            }

            return await Task.FromResult(response);
        }
    }
}
