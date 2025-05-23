using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class SelectDepartamentoQueryHandler : SearchQueryHandlerBase<SelectDepartamentoQuery, SelectDepartamentoFilterDto, SelectDepartamentoDto>
    {
        private readonly IRepository<Entity.Models.Departamento> _DepartamentoRepository;

        public SelectDepartamentoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Departamento> DepartamentoRepository
        ) : base(mapper)
        {
            _DepartamentoRepository = DepartamentoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectDepartamentoDto>>> HandleQuery(SelectDepartamentoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectDepartamentoDto>>();

            Expression<Func<Entity.Models.Departamento, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            /*
            if (filters?.FechaDesde.HasValue == true || filters?.FechaHasta.HasValue == true)
            {
                if (filters?.FechaDesde.HasValue == true)
                {
                    var fechaDesde = filters.FechaDesde.GetStartDate();
                    filter = filter.And(x => x.Fecha >= fechaDesde);
                }

                if (filters?.FechaHasta.HasValue == true)
                {
                    var fechaHasta = filters.FechaHasta.GetEndDate();
                    filter = filter.And(x => x.Fecha < fechaHasta);
                }
            }
            */

            if (filters?.IdDepartamento.HasValue == true)
                filter = filter.And(x => x.IdDepartamento == filters.IdDepartamento);

            var sorts = new List<SortExpression<Entity.Models.Departamento>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Departamento>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Departamentos = await _DepartamentoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var DepartamentoDtos = _mapper?.Map<IEnumerable<SelectDepartamentoDto>>(Departamentos.Items);

            var searchResult = new SearchResultDto<SelectDepartamentoDto>(
                DepartamentoDtos ?? new List<SelectDepartamentoDto>(),
                Departamentos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
