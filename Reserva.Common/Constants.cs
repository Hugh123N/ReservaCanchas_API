namespace Reserva.Common
{
    public class Constants
    {
        public struct Security
        {
            public struct User
            {
                public const string Admin = "Admin";
            }
        }

        public struct Role
        {
            public const string Admin = "ADMIN";
            public const string Proveedor = "PROVEEDOR";
            public const string Cliente = "CLIENTE";
            public const string Operador = "OPERADOR";
        }
        public struct RoleIds
        {
            public const string Admin = "1873C70C-ABD0-F011-A1DD-58FB84C3C26E";
            public const string Proveedor = "1973C70C-ABD0-F011-A1DD-58FB84C3C26E";
            public const string Cliente = "1B73C70C-ABD0-F011-A1DD-58FB84C3C26E";
            public const string Operador = "1A73C70C-ABD0-F011-A1DD-58FB84C3C26E";
        }
        public struct ESTADO_PROVEEDOR
        {
            public const string Pendiente = "01";
            public const string Aprobado = "02";
            public const string Rechazado = "03";
        }
        public struct ESTADO_USUARIO
        {
            public const string Activo = "01";
            public const string Inactivo = "02";
            public const string Suspendido = "03";
        }

        public struct ESTADO_CANCHA
        {
            public const string Aprobado = "01";
            public const string Pendiente = "02";
            public const string Rechazado = "03";
            public const string Suspendido = "04";
            public const string Mantenimiento = "05";
        }

        public struct TIPO_VALIDACION
        {
            public const string GOOGLE = "Google";
            public const string FACEBOOK = "Facebook";
            public const string CORREO = "Correo";
            public const string APPLE = "Apple";
        }

        public struct ESTADO_RESERVA
        {
            public const string Pendiente = "01";
            public const string Confirmado = "02";
            public const string Cancelado = "03";
            public const string Expirado = "04";
        }

        public struct ESTADO_PAGO
        {
            public const string Pagado = "01";
            public const string Pendiente = "02";
            public const string Rechazado = "03";
            public const string Parcial = "04"; // Para adelantos en efectivo
        }

        public struct METODO_PAGO
        {
            public const string Tarjeta = "01";
            public const string Efectivo = "02";
            public const string Transferencia = "03";
            public const string Yape = "04";
            public const string Plin = "05";
        }

    }
}
