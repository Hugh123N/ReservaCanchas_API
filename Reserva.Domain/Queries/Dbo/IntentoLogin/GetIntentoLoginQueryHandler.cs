using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.IntentoLogin;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.IntentoLogin
{
    public class GetIntentoLoginQueryHandler : QueryHandlerBase<GetIntentoLoginQuery, GetIntentoLoginDto>
    {
        private readonly IRepository<Entity.IntentoLogin> _IntentoLoginRepository;

        public GetIntentoLoginQueryHandler(
            IMapper mapper,
            GetIntentoLoginQueryValidator validator,
            IRepository<Entity.IntentoLogin> IntentoLoginRepository
        ) : base(mapper, validator)
        {
            _IntentoLoginRepository = IntentoLoginRepository;
        }

        protected override async Task<ResponseDto<GetIntentoLoginDto>> HandleQuery(GetIntentoLoginQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetIntentoLoginDto>();
            var IntentoLogin = await _IntentoLoginRepository.GetByAsync(x => x.IdIntentoLogin == request.Id);
            var IntentoLoginDto = _mapper?.Map<GetIntentoLoginDto>(IntentoLogin);

            if (IntentoLogin != null && IntentoLoginDto != null)
            {
                response.UpdateData(IntentoLoginDto);
            }

            return await Task.FromResult(response);
        }
    }
}
