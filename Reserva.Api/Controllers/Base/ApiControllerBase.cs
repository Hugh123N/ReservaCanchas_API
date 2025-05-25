using Microsoft.AspNetCore.Mvc;

namespace Reserva.Api.Controllers.Base
{
    public class ApiControllerBase : ControllerBase
    {
        private readonly IConfiguration? _configuration;
        private readonly IWebHostEnvironment? _webHostEnvironment;

        public ApiControllerBase(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetService<IConfiguration>();
            _webHostEnvironment = serviceProvider.GetService<IWebHostEnvironment>();
        }

    }
}
