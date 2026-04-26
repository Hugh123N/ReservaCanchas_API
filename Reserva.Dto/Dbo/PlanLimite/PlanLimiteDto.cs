using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.PlanLimite
{
    public class PlanLimiteDto
    {
        public int IdPlane { get; set; }
        public string Codigo { get; set; } = null!;
        public int Valor { get; set; }
    }
}
