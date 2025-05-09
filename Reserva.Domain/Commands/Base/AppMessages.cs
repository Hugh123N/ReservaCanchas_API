using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Base
{
    internal class AppMessages
    {
        public const string NotFound = "El registro no fue encontrado.";
        public const string CreateSuccess = "Registro creado exitosamente.";
        public const string UpdateSuccess = "Registro actualizado correctamente.";
        public const string DeleteSuccess = "Registro eliminado correctamente.";
    }
}
