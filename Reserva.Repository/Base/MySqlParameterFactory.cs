using MySqlConnector;
using Reserva.Repository.Abstractions.Base;
using System.Data;

namespace Reserva.Repository.Base
{
    public class MySqlParameterFactory : IDbParameterFactory
    {
        public IDbDataParameter CreateParameter(string name, object value)
        {
            // MySQL acepta parámetros con @ pero el nombre debe coincidir con el parámetro del SP
            // Si el nombre viene con @, MySQL lo maneja correctamente
            return new MySqlParameter(name, value);
        }

        public IDbDataParameter CreateOutputParameter(string name, DbType dbType, int size)
        {
            return new MySqlParameter
            {
                ParameterName = name,
                DbType = dbType,
                Size = size,
                Direction = ParameterDirection.Output
            };
        }

    }
}
