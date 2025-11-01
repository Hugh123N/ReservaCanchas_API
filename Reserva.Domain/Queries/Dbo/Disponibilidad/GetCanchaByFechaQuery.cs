using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Disponibilidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.Disponibilidad
{
    public class GetCanchaByFechaQuery : QueryBase<List<string>>
    {
        public GetCanchaByFechaQuery(DateTimeOffset fecha, int canchaId)
        {
            Fecha = fecha;
            CanchaId = canchaId;
        }
        public DateTimeOffset Fecha { get; }
        public int CanchaId { get; }
    }
}
