using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class CrearUbigeoAutoCommand : CommandBase<GetUbigeoDto>
    {
        public CrearUbigeoAutoCommand(string departamento, string provincia, string distrito)
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
