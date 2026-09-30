using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Usuario;
using System;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class GetUserByEmailQuery : QueryBase<CheckEmailResultDto>
    {
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}