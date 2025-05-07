using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Rol;
using Reserva.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.Rol
{
    public class SelectComboRolQueryHandler : QueryHandlerBase<SelectComboRolQuery, IEnumerable<SelectComboRolDto>>
    {
        private readonly ReservaCanchasContext _context;

        public SelectComboRolQueryHandler(
            IMapper mapper,
            ReservaCanchasContext context) : base(mapper)
        {
            _context = context;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboRolDto>>> HandleQuery(SelectComboRolQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboRolDto>>();
            var list = await _context.Rols.AsNoTracking().ToListAsync(cancellationToken);

            var listDtos = _mapper?.Map<IEnumerable<SelectComboRolDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboRolDto>());

            return response;
        }
    }
}
