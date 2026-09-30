using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class GetUserByEmailQueryHandler : QueryHandlerBase<GetUserByEmailQuery, CheckEmailResultDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;

        public GetUserByEmailQueryHandler(
            IMapper mapper,
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.Operador> operadorRepository
        ) : base(mapper, mediator)
        {
            _userManager = userManager;
            _proveedorRepository = proveedorRepository;
            _operadorRepository = operadorRepository;
        }

        protected override async Task<ResponseDto<CheckEmailResultDto>> HandleQuery(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<CheckEmailResultDto>();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.UpdateData(new CheckEmailResultDto
                {
                    Status = CheckEmailStatus.NoExiste,
                    Email = request.Email
                });
                return response;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleList = roles.ToList();

            var status = roles.Any(x => string.Equals(x, Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase))
               ? CheckEmailStatus.ExisteConProveedor
               : CheckEmailStatus.ExisteSinProveedor;

            response.UpdateData(new CheckEmailResultDto
            {
                Status = status,
                Email = user.Email,
                IdUser = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roleList
            });

            return response;
        }
    }
}