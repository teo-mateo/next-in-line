using System.Data;
using Dapper;
using MediatR;
using NextInLine.Server.Exceptions;

namespace NextInLine.Server.CQRS.Commands;

public record CheckItemCommand(int ItemId) : IRequest<CheckItemResult>;

public record CheckItemResult(bool Success);

public class CheckItemCommandHandler : IRequestHandler<CheckItemCommand, CheckItemResult>
{
    private readonly IDbConnection _connection;

    public CheckItemCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<CheckItemResult> Handle(CheckItemCommand request, CancellationToken cancellationToken)
    {
        var itemExists = await _connection.QuerySingleOrDefaultAsync<int?>("SELECT Id FROM Items WHERE Id = @ItemId", new { ItemId = request.ItemId });

        if (!itemExists.HasValue)
        {
            throw new ItemNotFoundException(request.ItemId);
        }

        var sql = "UPDATE Items SET Checked = TRUE, WhenChecked = NOW() WHERE Id = @ItemId";
        var affectedRows = await _connection.ExecuteAsync(sql, new { request.ItemId });

        return new CheckItemResult(affectedRows > 0);
    }

}
