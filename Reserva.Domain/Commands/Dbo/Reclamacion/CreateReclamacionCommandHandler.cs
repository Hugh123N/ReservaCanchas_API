using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Reclamaciones;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Reclamacion
{
    public class CreateReclamacionCommandHandler : CommandHandlerBase<CreateReclamacionCommand, GetReclamacionDto>
    {
        private readonly IRepository<Entity.Reclamacion> _ReclamacionRepository;
        private readonly IConfiguration _configuration;

        public CreateReclamacionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateReclamacionCommandValidator validator,
            IRepository<Entity.Reclamacion> ReclamacionRepository,
            IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReclamacionRepository = ReclamacionRepository;
            _configuration = configuration;
        }

        public override async Task<ResponseDto<GetReclamacionDto>> HandleCommand(CreateReclamacionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReclamacionDto>();
            var reclamacion = _mapper!.Map<Entity.Reclamacion>(request.CreateDto);

            reclamacion.Nombres = reclamacion.Nombres.Trim();
            reclamacion.NroDoc = reclamacion.NroDoc.Trim();
            reclamacion.Domicilio = reclamacion.Domicilio.Trim();
            reclamacion.Email = reclamacion.Email.Trim();
            reclamacion.Telefono = ReclamacionReglas.Opcional(reclamacion.Telefono);
            reclamacion.Representante = reclamacion.EsMenor ? ReclamacionReglas.Opcional(reclamacion.Representante) : null;
            reclamacion.DescripcionServicio = reclamacion.DescripcionServicio.Trim();
            reclamacion.ReferenciaOperacion = ReclamacionReglas.Opcional(reclamacion.ReferenciaOperacion);
            reclamacion.Detalle = reclamacion.Detalle.Trim();
            reclamacion.Pedido = reclamacion.Pedido.Trim();
            reclamacion.FechaRegistro = DateTimeOffset.UtcNow;
            reclamacion.ComercioTitular = "Nuñez Paucar Hugo";
            reclamacion.ComercioNombre = "ReservaFast";
            reclamacion.ComercioRuc = "10733842356";
            reclamacion.ComercioDireccion = "Calle 19, Campoy, San Juan de Lurigancho, Lima, Perú";
            reclamacion.ComercioTelefono = "965147445";
            reclamacion.ComercioEmail = _configuration["LibroReclamaciones:EmailAtencion"] ?? "hughhnp@gmail.com";

            // La columna es normal y nullable: el ID se obtiene al insertar, no mediante una fórmula del motor SQL.
            reclamacion.NumeroHoja = null;
            await _ReclamacionRepository.AddAsync(reclamacion);

            var anioPeru = reclamacion.FechaRegistro.ToOffset(TimeSpan.FromHours(-5)).Year;
            reclamacion.NumeroHoja = $"RF-{reclamacion.IdReclamacion}-{anioPeru}";
            await _ReclamacionRepository.UpdateAsync(reclamacion);

            response.UpdateData(_mapper.Map<GetReclamacionDto>(reclamacion));
            response.AddOkResult("Hoja de reclamación registrada. Conserva tu constancia.");
            return response;
        }
    }
}
