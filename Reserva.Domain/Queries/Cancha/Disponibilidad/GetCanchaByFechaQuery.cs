using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class GetCanchaByFechaQuery : QueryBase<List<string>>
    {
        public GetCanchaByFechaQuery(DateTime fecha, int canchaId)
        {
            Fecha = fecha;
            CanchaId = canchaId;
        }
        public DateTime Fecha { get; }
        public int CanchaId { get; }
    }
}
