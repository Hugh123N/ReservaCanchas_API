using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class GetCanchaByFechaQuery : QueryBase<List<HorarioDisponibleDto>>
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
