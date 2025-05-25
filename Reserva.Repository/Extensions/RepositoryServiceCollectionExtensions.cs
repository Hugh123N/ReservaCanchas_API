using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
//using Reserva.Audit.RestClient.Base;
//using Reserva.Audit.RestClient.Extensions;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Base;
using Reserva.Repository.Transactions;
using Reserva.Repository.Utils;
using System.Reflection;
using System.Linq.Expressions;
using Reserva.Entity;
using Reserva.Entity.Models;

namespace Reserva.Repository.Extensions
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection UseRepositories(
            this IServiceCollection services,
            string connectionString,
            string urlAuditService,
            string mainProjectAssemblyName,
            Assembly customRepositoryAssembly = null!,
            TimeZoneInfo timeZoneInfo = null!
        )
        {
            services.AddSqlServer<ReservaCanchasContext>(connectionString, b => b.MigrationsAssembly(mainProjectAssemblyName));

            services.AddScoped<DbContext, ReservaCanchasContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork<DbContext>>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            if (customRepositoryAssembly != null)
            {
                services.Scan(selector => selector
                    .FromAssemblies(customRepositoryAssembly)
                    .AddClasses(x => x.Where(c => c.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                );
            }

            var timezoneInfoData = new TimezoneInfoData
            {
                TimeZone = timeZoneInfo ?? TimezoneUtils.GetEasternOrPacific()
            };

            services.AddSingleton(timezoneInfoData);

            //Audit
            //services.UseAuditServices(new ServiceOptions
            //{
            //    BaseUrl = urlAuditService
            //}, config =>
            //{
            //    config.LogTrace= true;
            //    config.AuditEntity(Array.Empty<Expression<Func<Lote, object>>>());
            //});

            return services;
        }
    }
}
