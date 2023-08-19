using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using MediatR;

namespace NextInLine.Server.CQRS.Commands;

public record AddItemCommand(
    [Required] string Name,
    [Required] string AddedBy, IEnumerable<int> TagIds) : IRequest<int>;

public class AddItemCommandHandler : IRequestHandler<AddItemCommand, int>
{
    private readonly IDbConnection _connection;

    public AddItemCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> Handle(AddItemCommand request, CancellationToken cancellationToken)
    {
        // Insert into Items table and get the newly generated ID
        var insertItemSql = @"INSERT INTO Items(Name, WhenAdded, AddedBy) 
                              VALUES (@Name, NOW(), @AddedBy) 
                              RETURNING Id;";

        var itemId = await _connection.QuerySingleAsync<int>(insertItemSql, new
        {
            request.Name,
            request.AddedBy
        });

        // Insert into ItemTags table
        if (request.TagIds.Any())
        {
            var insertItemTagsSql = @"INSERT INTO ItemTags(ItemId, TagId) VALUES (@ItemId, @TagId);";
            await _connection.ExecuteAsync(insertItemTagsSql, request.TagIds.Select(tagId => new
            {
                ItemId = itemId,
                TagId = tagId
            }));
        }

        return itemId; // return the ID of the newly inserted item
    }
}