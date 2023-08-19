using System.Data;
using Dapper;
using MediatR;
using NextInLine.Server.CQRS.Queries.Models;

namespace NextInLine.Server.CQRS.Queries;

public record GetTagsQuery : IRequest<IEnumerable<Tag>>;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, IEnumerable<Tag>>
{
    private readonly IDbConnection _connection;

    // Constructor: Accepts an IDbConnection as a dependency and stores it as a private field
    public GetTagsQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    // Handler method: Executes the query and returns the result
    public async Task<IEnumerable<Tag>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        // SQL query: Retrieves all tags sorted by their name
        const string sql = @"SELECT Id, TagName
                             FROM Tags
                             ORDER BY TagName";

        // Executes the SQL query and maps the results to Tag objects
        var tags = await _connection.QueryAsync<Tag>(sql);

        // Returns the list of tags
        return tags;
    }
}