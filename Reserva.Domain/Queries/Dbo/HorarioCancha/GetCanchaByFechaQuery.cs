using Reserva.Domain.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
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
