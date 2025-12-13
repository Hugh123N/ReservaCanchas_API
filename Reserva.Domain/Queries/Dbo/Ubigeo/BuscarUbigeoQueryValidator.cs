using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Ubigeo
{
    /// <summary>
    /// Validador para la búsqueda de ubigeo
    /// </summary>
    public class BuscarUbigeoQueryValidator : QueryValidatorBase<BuscarUbigeoQuery>
    {
        public BuscarUbigeoQueryValidator()
        {
            RequiredField(x => x.Departamento, Resources.Dbo.Ubigeo.Departamento);
            RequiredField(x => x.Provincia, Resources.Dbo.Ubigeo.Provincia);
            RequiredField(x => x.Distrito, Resources.Dbo.Ubigeo.Distrito);
        }
    }
}
