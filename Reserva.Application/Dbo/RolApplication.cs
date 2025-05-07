using Azure;
using MediatR;
using Reserva.Dto.Base;
using Reserva.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reserva.Application.Base;
using Reserva.Dto.Dbo.Rol;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Domain.Queries.Dbo.Rol;

namespace Reserva.Application.Dbo
{
    public class RolApplication : ApplicationBase, IRolApplication
    {
        public RolApplication(IMediator mediator) : base(mediator)
        {

        }

        public Task<ResponseDto<GetRolDto>> Create(CreateRolDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto<GetRolDto>> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto<IEnumerable<ListRolDto>>> List(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<IEnumerable<SelectComboRolDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboRolQuery());

        public Task<ResponseDto<GetRolDto>> Update(UpdateRolDto updateDto)
        {
            throw new NotImplementedException();
        }
    }
}
