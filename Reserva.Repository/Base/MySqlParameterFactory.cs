using MySql.Data.MySqlClient;
using Reserva.Repository.Abstractions.Base;
using System.Data;

namespace Reserva.Repository.Base
{
    public class MySqlParameterFactory : IDbParameterFactory
    {
        public IDbDataParameter CreateParameter(string name, object value)
        {
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
