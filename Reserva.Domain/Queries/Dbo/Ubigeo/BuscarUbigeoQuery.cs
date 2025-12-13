using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Domain.Queries.Dbo.Ubigeo
{
    public class BuscarUbigeoQuery : QueryBase<GetUbigeoDto>
    {
        public BuscarUbigeoQuery(string departamento, string provincia, string distrito)
        {
            Departamento = departamento;
            Provincia = provincia;
            Distrito = distrito;
        }

        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
    }
}
