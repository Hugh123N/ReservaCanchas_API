using Microsoft.Data.SqlClient;
using Reserva.Repository.Abstractions.Base;
using System.Data;

namespace Reserva.Repository.Base
{
    public class SqlServerParameterFactory : IDbParameterFactory
    {
        public IDbDataParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public IDbDataParameter CreateOutputParameter(string name, DbType dbType, int size)
        {
            return new SqlParameter
            {
                ParameterName = name,
                DbType = dbType,
                Size = size,
                Direction = ParameterDirection.Output
            };
        }

    }
}
