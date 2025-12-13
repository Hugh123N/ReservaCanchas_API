using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Ubigeo;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class CrearUbigeoAutoCommandHandler : CommandHandlerBase<CrearUbigeoAutoCommand, GetUbigeoDto>
    {
        private readonly IRepository<Entity.Ubigeo> _UbigeoRepository;

        public CrearUbigeoAutoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CrearUbigeoAutoCommandValidator validator,
            IRepository<Entity.Ubigeo> UbigeoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        public override async Task<ResponseDto<GetUbigeoDto>> HandleCommand(CrearUbigeoAutoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUbigeoDto>();

            var departamentoNorm = request.Departamento.Trim();
            var provinciaNorm = request.Provincia.Trim();
            var distritoNorm = request.Distrito.Trim();

            var departamento = await _UbigeoRepository.FindAll()
                .Where(x => x.Departamento != null &&
                           x.Departamento.ToLower() == departamentoNorm.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (departamento == null)
            {
                response.AddErrorResult(Resources.Dbo.Ubigeo.DepartamentoNoEncontrado);
                return response;
            }

            var codigoDepartamento = departamento.CodigoUbigeo.Substring(0, 2);

            string codigoProvincia;

            var provinciaExistente = await _UbigeoRepository.FindAll()
                .Where(x => x.CodigoUbigeo.StartsWith(codigoDepartamento) &&
                           x.Provincia != null &&
                           x.Provincia.ToLower() == provinciaNorm.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (provinciaExistente != null)
            {
                // Extraer los primeros 4 dígitos del código encontrado
                codigoProvincia = provinciaExistente.CodigoUbigeo.Substring(0, 4);
            }
            else
            {
                // Provincia no existe, generar nuevo código
                var provinciasDelDepartamento = await _UbigeoRepository.FindAll()
                    .Where(x => x.CodigoUbigeo.StartsWith(codigoDepartamento))
                    .Select(x => x.CodigoUbigeo)
                    .ToListAsync(cancellationToken);

                int maxNumeroProvincia = 0;
                if (provinciasDelDepartamento.Any())
                {
                    // Extraer dígitos 3-4 de cada código y buscar el máximo
                    maxNumeroProvincia = provinciasDelDepartamento
                        .Select(codigo => int.Parse(codigo.Substring(2, 2)))
                        .Max();
                }

                var nuevoNumeroProvincia = maxNumeroProvincia + 1;
                codigoProvincia = codigoDepartamento + nuevoNumeroProvincia.ToString("D2");
            }

            var distritosDelaProvincia = await _UbigeoRepository.FindAll()
                .Where(x => x.CodigoUbigeo.StartsWith(codigoProvincia))
                .Select(x => x.CodigoUbigeo)
                .ToListAsync(cancellationToken);

            int maxNumeroDistrito = 0;
            if (distritosDelaProvincia.Any())
            {
                // Extraer dígitos 5-6 de cada código y buscar el máximo
                maxNumeroDistrito = distritosDelaProvincia
                    .Select(codigo => int.Parse(codigo.Substring(4, 2)))
                    .Max();
            }

            var nuevoNumeroDistrito = maxNumeroDistrito + 1;
            var codigoUbigeoCompleto = codigoProvincia + nuevoNumeroDistrito.ToString("D2");

            var nuevoUbigeo = new Entity.Ubigeo
            {
                CodigoUbigeo = codigoUbigeoCompleto,
                Departamento = departamento.Departamento,
                Provincia = provinciaNorm,
                Distrito = distritoNorm
            };

            await _UbigeoRepository.AddAsync(nuevoUbigeo);
            await _UbigeoRepository.SaveAsync();

            var ubigeoDto = _mapper?.Map<GetUbigeoDto>(nuevoUbigeo);
            if (ubigeoDto != null) response.UpdateData(ubigeoDto);

            response.AddOkResult(string.Format(Resources.Dbo.Ubigeo.UbigeoCreado, codigoUbigeoCompleto));

            return response;
        }
    }
}
