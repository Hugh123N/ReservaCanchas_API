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
    }
}
