using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Resources.Dbo;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Operador;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class GetOperadorQueryHandler : QueryHandlerBase<GetOperadorQuery, GetOperadorDto>
    {
        private readonly UserManager<Entity.ApplicationUser> _UsuarioManager;
        private readonly IRepository<Entity.Operador> _OperadorRepository;

        public GetOperadorQueryHandler(
            IMapper mapper,
            GetOperadorQueryValidator validator,
            UserManager<Entity.ApplicationUser> userManager,
            IRepository<Entity.Operador> OperadorRepository
        ) : base(mapper, validator)
        {
            _UsuarioManager = userManager;
            _OperadorRepository = OperadorRepository;
        }

        protected override async Task<ResponseDto<GetOperadorDto>> HandleQuery(GetOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetOperadorDto>();
            var Operador = await _OperadorRepository.GetByAsync(x => x.IdOperador == request.Id, 
                x => x.OperadorCancha.Where(x => x.Activo));
            
            var operadorDto = _mapper?.Map<GetOperadorDto>(Operador);

            if (Operador != null && operadorDto != null)
            {
                var user = await _UsuarioManager.FindByIdAsync(Operador.IdUsuario);

                operadorDto.Imagen = user?.Imagen ?? string.Empty;
                operadorDto.FechaCreacion = Operador.CreateDate;
                operadorDto.FechaActualizacion = Operador.UpdateDate;
                operadorDto.CanchaIds = Operador.OperadorCancha.Select(oc => oc.IdCancha).ToList();

                response.UpdateData(operadorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
