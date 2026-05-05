using System.Data;
using Dapper;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.TypeHandlers;

// Lets Dapper store the enum as a readable string in SQL Server.
// This keeps the database human-friendly and easy to inspect in SSMS.
public sealed class OrderStatusTypeHandler : SqlMapper.TypeHandler<OrderStatus>
{
    public override void SetValue(IDbDataParameter parameter, OrderStatus value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString();
    }

    public override OrderStatus Parse(object value)
    {
        return Enum.Parse<OrderStatus>(value.ToString() ?? "Pending", ignoreCase: true);
    }
}
