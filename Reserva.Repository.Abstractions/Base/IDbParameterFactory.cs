using Microsoft.Data.SqlClient;
using Reserva.Entity.Base;
using System.Data;
using System.Linq.Expressions;

namespace Reserva.Repository.Abstractions.Base
{
    public interface IDbParameterFactory
    {
        IDbDataParameter CreateParameter(string name, object value);
        IDbDataParameter CreateOutputParameter(string name, DbType dbType, int size);
    }
}
