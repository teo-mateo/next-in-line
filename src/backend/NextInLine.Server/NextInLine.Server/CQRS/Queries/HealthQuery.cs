using System.Data;
using Dapper;
using MediatR;

namespace NextInLine.Server.CQRS.Queries;

public record HealthQuery : IRequest<HealthResult>;

public record HealthResult(bool IsHealthy, string Message = "");

public class HealthQueryHandler : IRequestHandler<HealthQuery, HealthResult>
{
    private readonly IDbConnection _connection;

    public HealthQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<HealthResult> Handle(HealthQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _connection.QuerySingleAsync<string>("SELECT 1");
            return new HealthResult(true, "Ok");
        }
        catch (Exception ex)
        {
            return new HealthResult(false, ex.Message);
        }
    }
}