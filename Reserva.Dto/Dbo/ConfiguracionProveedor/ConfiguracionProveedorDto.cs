using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.ConfiguracionProveedor;

public class ConfiguracionProveedorDto
{

    public int IdProveedor { get; set; }

    public int? DuracionPreReserva { get; set; }

    public decimal PorcentajeAdelantoMinimo { get; set; }

    public int TiempoLimiteCancelacion { get; set; }

    public decimal PorcentajeDevolucionCompleto { get; set; }

    public decimal PorcentajeDevolucionParcial { get; set; }

}
