using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Dbo.Ubigeo;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Ubigeo;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Ubigeo
{
    /// <summary>
    /// Handler para buscar ubigeo. Si no existe, ejecuta el comando de creación automática
    /// Implementa el patrón OPCIÓN A: Query que llama a Command internamente
    /// </summary>
    public class BuscarUbigeoQueryHandler : QueryHandlerBase<BuscarUbigeoQuery, GetUbigeoDto>
    {
        private readonly IRepository<Entity.Ubigeo> _UbigeoRepository;

        public BuscarUbigeoQueryHandler(
            IMapper mapper,
            BuscarUbigeoQueryValidator validator,
            IRepository<Entity.Ubigeo> UbigeoRepository,
            IMediator mediator
        ) : base(mapper, mediator, validator)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        protected override async Task<ResponseDto<GetUbigeoDto>> HandleQuery(BuscarUbigeoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUbigeoDto>();

            var departamentoNorm = request.Departamento.Trim();
            var provinciaNorm = request.Provincia.Trim();
            var distritoNorm = request.Distrito.Trim();

            var ubigeoExistente = await _UbigeoRepository.FindAll()
                .Where(x => x.Departamento != null && x.Provincia != null && x.Distrito != null &&
                           x.Departamento.ToLower() == departamentoNorm.ToLower() &&
                           x.Provincia.ToLower() == provinciaNorm.ToLower() &&
                           x.Distrito.ToLower() == distritoNorm.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (ubigeoExistente != null)
            {
                var ubigeoDto = _mapper?.Map<GetUbigeoDto>(ubigeoExistente);
                if (ubigeoDto != null) response.UpdateData(ubigeoDto);
                response.AddOkResult(Resources.Dbo.Ubigeo.UbigeoEncontrado);
                return response;
            }

            var createCommand = new CrearUbigeoAutoCommand(
                departamentoNorm,
                provinciaNorm,
                distritoNorm
            );

            var resultadoCreacion = await _mediator!.Send(createCommand, cancellationToken);
            
            return resultadoCreacion;
        }
    }
}
