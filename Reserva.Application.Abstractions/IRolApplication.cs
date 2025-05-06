using Azure;
using Reserva.Dto.Base;
using Reserva.Dto.Rol;
using Reserva.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Application.Abstractions
{
    public interface IRolApplication
    {
        Task<ResponseDto<GetRolDto>> Create(CreateRolDto createDto);
        Task<ResponseDto<GetRolDto>> Update(UpdateRolDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetRolDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListRolDto>>> List(int id);
        Task<ResponseDto<IEnumerable<SelectComboRolDto>>> SelectCombo();
        

    }
}
