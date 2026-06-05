using AccpacGraphqlClean.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccpacGraphqlClean.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAccpacEndpointCatalog, AccpacRestEndpointCatalog>();
        services.AddScoped<IAccpacOperationExecutor, AccpacOperationExecutor>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddDbContext<SettingsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SettingsDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "Data Source=settings.db";
            }

            options.UseSqlite(connectionString);
        });

        services.AddScoped<IUserCredentialValidator, EfUserCredentialValidator>();
        services.AddScoped<ICompanyConnectionDetailsProvider, EfCompanyConnectionDetailsProvider>();
        services.AddScoped<IApVendorService, Sage300ApVendorService>();
        services.AddScoped<IApVendorGroupService, Sage300ApVendorGroupService>();
        services.AddScoped<IApPaymentCodeService, Sage300ApPaymentCodeService>();
        services.AddScoped<IApPaymentTermsService, Sage300ApPaymentTermsService>();
        services.AddScoped<IApRemitToLocationsService, Sage300ApRemitToLocationsService>();
        services.AddScoped<IApRecurringPayablesService, Sage300ApRecurringPayablesService>();
        services.AddScoped<IApInvoiceService, Sage300ApInvoiceService>();
        services.AddScoped<IApPaymentService, Sage300ApPaymentService>();
        services.AddScoped<IApAdjustmentService, Sage300ApAdjustmentService>();
        services.AddScoped<IArInvoiceService, Sage300ArInvoiceService>();
        services.AddScoped<IArAdjustmentService, Sage300ArAdjustmentService>();

        return services;
    }
}
