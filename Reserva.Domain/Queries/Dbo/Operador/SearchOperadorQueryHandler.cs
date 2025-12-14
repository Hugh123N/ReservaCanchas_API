using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Queries.Dbo.HorarioCancha;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Dbo.Operador;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Entity.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class SearchOperadorQueryHandler : SearchQueryHandlerBase<SearchOperadorQuery, SearchOperadorFilterDto, SearchOperadorDto>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;
        private readonly IRepository<Entity.Cancha> _CanchaRepository;

        public SearchOperadorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Operador> OperadorRepository,
            IRepository<Entity.Cancha> CanchaRepository
        ) : base(mapper)
        {
            _OperadorRepository = OperadorRepository;
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchOperadorDto>>> HandleQuery(SearchOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchOperadorDto>>();

            Expression<Func<Entity.Operador, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            filter = filter.And(x => x.Activo == true);

            if (filters.IdProveedor.HasValue)
                filter = filter.And(x => x.IdProveedor == filters.IdProveedor.Value);

            if (!string.IsNullOrEmpty(filters.Nombre))
                filter = filter.And(x => x.IdUsuarioNavigation.FirstName!.Contains(filters.Nombre) || x.IdUsuarioNavigation.LastName.Contains(filters.Nombre));
            
            if (!string.IsNullOrEmpty(filters.Email))
                filter = filter.And(x => x.IdUsuarioNavigation!.Email!.Contains(filters.Email));

            if (!string.IsNullOrEmpty(filters.Telefono))
                filter = filter.And(x => x.IdUsuarioNavigation!.PhoneNumber!.Contains(filters.Telefono));
            

            var sorts = new List<SortExpression<Entity.Operador>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Operador>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Operadors = await _OperadorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter,
                x => x.IdUsuarioNavigation,
                x => x.OperadorCancha.Where(oc => oc.Activo)
            );

            var OperadorDtos = Operadors.Items.Where(x => x.IdUsuarioNavigation != null)
                .Select(operador => new SearchOperadorDto { 
                    IdOperador = operador.IdOperador,
                    Nombre = operador.IdUsuarioNavigation!.FirstName,
                    Apellidos = operador.IdUsuarioNavigation!.LastName,
                    Email = operador.IdUsuarioNavigation!.Email!,
                    Telefono = operador.IdUsuarioNavigation!.PhoneNumber!,
                    FechaCreacion = operador.CreateDate
                }).ToList();

            foreach (var it in OperadorDtos)
            {
                var nombresCancha = new List<string>();
                var operadorEntity = Operadors.Items.FirstOrDefault(x => x.IdOperador == it.IdOperador);

                var idsCancha = operadorEntity.OperadorCancha.Select(x => x.IdCancha).ToList();

                if (idsCancha.Count() > 0) {
                    var canchas = await _CanchaRepository.FindByAsNoTrackingAsync(x => idsCancha.Contains(x.IdCancha) && x.Activo);
                    nombresCancha = canchas.Where(x => x.Nombre != null).Select(x => x.Nombre!).ToList();
                }

                it.Canchas = nombresCancha;
            }

            var searchResult = new SearchResultDto<SearchOperadorDto>(
                OperadorDtos ?? new List<SearchOperadorDto>(),
                Operadors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
