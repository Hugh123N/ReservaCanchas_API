using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services
{
    public interface IPlanLimitValidationService
    {
        Task<bool> CanCreateCanchaAsync(int idProveedor);
        Task<bool> CanCreateOperadorAsync(int idProveedor);
        Task<int> GetCanchaCountAsync(int idProveedor);
        Task<int> GetOperadorCountAsync(int idProveedor);
        Task<int?> GetLimiteAsync(int idProveedor, string codigoLimite);
        Task<string?> GetPlanNombreAsync(int idProveedor);
    }

    public class PlanLimitValidationService : IPlanLimitValidationService
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;

        public PlanLimitValidationService(
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.Operador> operadorRepository)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _canchaRepository = canchaRepository;
            _operadorRepository = operadorRepository;
        }

        public async Task<bool> CanCreateCanchaAsync(int idProveedor)
        {
            var limite = await GetLimiteAsync(idProveedor, "MAX_CANCHAS");
            if (limite == null) return true;
            if (limite.Value >= 999) return true;

            var count = await GetCanchaCountAsync(idProveedor);
            return count < limite.Value;
        }

        public async Task<bool> CanCreateOperadorAsync(int idProveedor)
        {
            var limite = await GetLimiteAsync(idProveedor, "MAX_OPERADORES");
            if (limite == null) return true;
            if (limite.Value >= 999) return true;

            var count = await GetOperadorCountAsync(idProveedor);
            return count < limite.Value;
        }

        public async Task<int> GetCanchaCountAsync(int idProveedor)
        {
            return await _canchaRepository.FindAll()
                .CountAsync(x => x.IdProveedor == idProveedor && x.Activo);
        }

        public async Task<int> GetOperadorCountAsync(int idProveedor)
        {
            return await _operadorRepository.FindAll()
                .CountAsync(x => x.IdProveedor == idProveedor && x.Activo);
        }

        public async Task<int?> GetLimiteAsync(int idProveedor, string codigoLimite)
        {
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                x => x.IdProveedor == idProveedor && x.EsActual && x.Activo,
                x => x.IdPlaneNavigation,
                x => x.IdPlaneNavigation.PlanLimite
            );

            if (proveedorPlan?.IdPlaneNavigation?.PlanLimite == null)
                return null;

            var limite = proveedorPlan.IdPlaneNavigation.PlanLimite
                .FirstOrDefault(l => l.Codigo == codigoLimite && l.Activo);

            return limite?.Valor;
        }

        public async Task<string?> GetPlanNombreAsync(int idProveedor)
        {
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                x => x.IdProveedor == idProveedor && x.EsActual && x.Activo,
                x => x.IdPlaneNavigation
            );

            return proveedorPlan?.IdPlaneNavigation?.Nombre;
        }
    }
}
