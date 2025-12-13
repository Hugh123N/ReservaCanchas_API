using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    /// <summary>
    /// Validador para el comando de creación automática de ubigeo
    /// </summary>
    public class CrearUbigeoAutoCommandValidator : CommandValidatorBase<CrearUbigeoAutoCommand>
    {
        public CrearUbigeoAutoCommandValidator()
        {
            RequiredField(x => x.Departamento, Resources.Dbo.Ubigeo.Departamento);
            RequiredField(x => x.Provincia, Resources.Dbo.Ubigeo.Provincia);
            RequiredField(x => x.Distrito, Resources.Dbo.Ubigeo.Distrito);
        }
    }
}
