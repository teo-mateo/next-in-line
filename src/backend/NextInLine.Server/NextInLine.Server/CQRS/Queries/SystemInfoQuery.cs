using MediatR;

namespace NextInLine.Server.CQRS.Queries;

public record SystemInfoQuery : IRequest<SystemInfoResult>;

public record SystemInfoResult(string Status);

public class SystemInfoQueryHandler : IRequestHandler<SystemInfoQuery, SystemInfoResult>
{
    public Task<SystemInfoResult> Handle(SystemInfoQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SystemInfoResult("OK"));
    }
}