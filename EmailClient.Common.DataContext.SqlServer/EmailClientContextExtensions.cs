using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Common.DataContext.SqlServer;

/// <summary>
/// Extension methods to register context to client applications DI container
/// </summary>
public static class EmailClientContextExtensions
{
    /// <summary>
    /// Extension method to register the EmailClientContext.
    /// </summary>
    /// <param name="services">The services container</param>
    /// <param name="connectionString">The DB connection string, default provided</param>
    /// <returns>The service container with registered service</returns>
    //if connection string is not passed to extension methods, create a default DB
    public static IServiceCollection AddEmailClientContext(
        this IServiceCollection services, string connectionString =
        "Data Source=.;Initial Catalog=EmailClient;"
        + "Integrated Security=true;MultipleActiveResultsets=true;")
    {
        services.AddDbContext<EmailClientContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    /// <summary>
    /// Extension method for registering a transient EmailClientContent.
    /// </summary>
    /// <param name="services">The services container</param>
    /// <param name="connectionString">The DB connection string, default provided</param>
    /// <returns>The service container with registered service</returns>
    //some instances, a transient service is required
    public static IServiceCollection AddEmailClientContextTransient(
        this IServiceCollection services, string connectionString =
        "Data Source=.;Initial Catalog=EmailClient;"
        + "Integrated Security=true;MultipleActiveResultsets=true;")
    {
        services.AddDbContext<EmailClientContext>(options =>
            options.UseSqlServer(connectionString), ServiceLifetime.Transient);

        return services;
    }
}
