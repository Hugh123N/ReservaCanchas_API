using Azure;
using MediatR;
using Reserva.Application.Abstractions;
using Reserva.Domain.Queries.Rol;
using Reserva.Dto.Base;
using Reserva.Dto.Rol;
using Reserva.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reserva.Application.Base;

namespace Reserva.Application
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
