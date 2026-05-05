using Dapper;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.TypeHandlers;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderInfrastructure(this IServiceCollection services)
    {
        // Register a Dapper type handler once at startup.
        SqlMapper.AddTypeHandler(new OrderStatusTypeHandler());

        services.AddSingleton<SqlConnectionFactory>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
