using System.Data;
using Dapper;
using MediatR;
using NextInLine.Server.CQRS.Queries.Models;
using NextInLine.Server.Exceptions;

namespace NextInLine.Server.CQRS.Queries;

public record GetItemQuery(int Id) : IRequest<Item>;

public class GetItemQueryHandler : IRequestHandler<GetItemQuery, Item>
{
    private readonly IDbConnection _connection;

    public GetItemQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Item> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        const string itemSql = @"SELECT Id, Name, WhenAdded, AddedBy, Checked, WhenChecked FROM Items WHERE Id = @Id";
        var item = await _connection.QuerySingleOrDefaultAsync<Item>(itemSql, new { Id = request.Id });

        if (item == null)
        {
            throw new ItemNotFoundException(request.Id);
        }

        const string tagsSql = @"SELECT t.Id, t.TagName
                                 FROM Tags t
                                 INNER JOIN ItemTags it ON t.Id = it.TagId
                                 WHERE it.ItemId = @Id";
        var tags = (await _connection.QueryAsync<Tag>(tagsSql, new { Id = request.Id })).ToList();

        item.Tags = tags;
        return item;
    }
}