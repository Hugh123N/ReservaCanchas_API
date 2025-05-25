using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Ubigeo
{
    public class UbigeoDto
    {
        public string CodigoUbigeo { get; set; } = null!;
        public string Departamento { get; set; } = null!;
        public string Provincia { get; set; } = null!;
        public string Distrito { get; set; } = null!;
    }
}
