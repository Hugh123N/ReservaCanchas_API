using MediatR;
using Reserva.Dto.Base;

namespace Reserva.Domain.Commands.Base
{
    public interface ICommand { }

    public class CommandBase : IRequest<ResponseDto>, ICommand
    {

    }

    public class CommandBase<TResponse> : IRequest<ResponseDto<TResponse>>, ICommand
    {

    }
}
