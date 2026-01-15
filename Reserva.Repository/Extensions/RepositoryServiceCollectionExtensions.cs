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
using Reserva.Repository.Data;

namespace Reserva.Repository.Extensions
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection UseRepositories(
            this IServiceCollection services,
            string connectionString,
            string urlAuditService,
            string mainProjectAssemblyName,
            bool isProduction,
            Assembly customRepositoryAssembly = null!,
            TimeZoneInfo timeZoneInfo = null!
        )
        {
            // Configurar DbContext según el ambiente
            if (isProduction)
                services.AddDbContext<ReservaCanchasContext>(options => options.UseSqlServer(connectionString,
                        b => b.MigrationsAssembly(mainProjectAssemblyName)));
            else
                services.AddDbContext<ReservaCanchasContext>(options => options.UseMySql(connectionString,
                        ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly(mainProjectAssemblyName)));

            // Registrar factory de parámetros según el ambiente
            if (isProduction)
                services.AddScoped<IDbParameterFactory, SqlServerParameterFactory>();
            else
                services.AddScoped<IDbParameterFactory, MySqlParameterFactory>();

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

            return services;
        }
    }
}
