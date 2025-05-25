using MediatR;
using Reserva.Dto.Base;

namespace Reserva.Domain.Commands.Base
{
    public class CommandBase : IRequest<ResponseDto>
    {

    }

    public class CommandBase<TResponse> : IRequest<ResponseDto<TResponse>>
    {

    }
}
