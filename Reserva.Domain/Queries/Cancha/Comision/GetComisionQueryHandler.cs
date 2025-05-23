using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class GetComisionQueryHandler : QueryHandlerBase<GetComisionQuery, GetComisionDto>
    {
        private readonly IRepository<Entity.Models.Comision> _ComisionRepository;

        public GetComisionQueryHandler(
            IMapper mapper,
            GetComisionQueryValidator validator,
            IRepository<Entity.Models.Comision> ComisionRepository
        ) : base(mapper, validator)
        {
            _ComisionRepository = ComisionRepository;
        }

        protected override async Task<ResponseDto<GetComisionDto>> HandleQuery(GetComisionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetComisionDto>();
            var Comision = await _ComisionRepository.GetByAsync(x => x.IdComision == request.Id);
            var ComisionDto = _mapper?.Map<GetComisionDto>(Comision);

            if (Comision != null && ComisionDto != null)
            {
                response.UpdateData(ComisionDto);
            }

            return await Task.FromResult(response);
        }
    }
}
