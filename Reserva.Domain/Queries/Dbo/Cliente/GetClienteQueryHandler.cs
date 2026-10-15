using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cliente;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Cliente
{
    public class GetClienteQueryHandler : QueryHandlerBase<GetClienteQuery, GetClienteDto>
    {
        private readonly IRepository<Entity.Cliente> _ClienteRepository;

        public GetClienteQueryHandler(
            IMapper mapper,
            GetClienteQueryValidator validator,
            IRepository<Entity.Cliente> ClienteRepository
        ) : base(mapper, validator)
        {
            _ClienteRepository = ClienteRepository;
        }

        protected override async Task<ResponseDto<GetClienteDto>> HandleQuery(GetClienteQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetClienteDto>();

            var cliente = await _ClienteRepository.GetByAsync(x => x.IdCliente == request.Id && x.Activo);

            var clienteDto = _mapper?.Map<GetClienteDto>(cliente);
            if (cliente != null && clienteDto != null) response.UpdateData(clienteDto);

            return await Task.FromResult(response);
        }
    }
}
