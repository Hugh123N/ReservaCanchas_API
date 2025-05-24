using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Ubigeo;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Queries.Cancha.Ubigeo
{
    public class ListUbigeoQueryHandler : QueryHandlerBase<ListUbigeoQuery, IEnumerable<DepartamentoDto>>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _repository;

        public ListUbigeoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Ubigeo> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<DepartamentoDto>>> HandleQuery(ListUbigeoQuery request, CancellationToken cancellationToken)
        {
                var ubigeos = await _repository.FindAllAsNoTracking().ToListAsync();

                var departamentos = ubigeos.Select(x =>
                        new DepartamentoDto
                        {
                            Codigo = x.CodigoUbigeo.Substring(0, 2),
                            Nombre = x.Departamento
                        })
                    .DistinctBy(x => x.Codigo).ToList();

                foreach (var departamento in departamentos)
                {
                    departamento.Provincias = ubigeos
                        .Where(x => x.CodigoUbigeo.StartsWith(departamento.Codigo!))
                        .Select(x =>
                        new ProvinciaDto
                        {
                            Codigo = x.CodigoUbigeo.Substring(0, 4),
                            Nombre = x.Provincia
                        })
                        .DistinctBy(x => x.Codigo).ToList();

                    foreach (var provincia in departamento.Provincias)
                    {
                        provincia.Distritos = ubigeos
                        .Where(x => x.CodigoUbigeo.StartsWith(provincia.Codigo!))
                        .Select(x =>
                        new DistritoDto
                        {
                            Codigo = x.CodigoUbigeo,
                            Nombre = x.Distrito
                        })
                        .DistinctBy(x => x.Codigo).ToList();
                    }
                }

                var response = new ResponseDto<IEnumerable<DepartamentoDto>>();

                response.UpdateData(departamentos);

                return response;
        }
    }
}
