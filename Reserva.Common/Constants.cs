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
            public const string Admin = "Admin";
        }

        public struct CODIGO_RANDOM_OPERACION
        {
            public const string REMUESTREO = "REM";
            public const string REENSAYO = "RFC";
        }

        public struct LIQUIDACION_ESTADO
        {
            public const string EN_PROCESO = "01";
            public const string COMPLETADO = "02";
            public const string ANULADO = "03";
        }

        public struct LIQUIDACION_TIPO
        {
            public const string COMPRA = "01";
            public const string REINTEGRO = "02";
        }
    }
}
