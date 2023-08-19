using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using MediatR;
using NextInLine.Server.CQRS.Queries.Models;

namespace NextInLine.Server.CQRS.Queries;

public record GetUncheckedItemsByTagQuery(int TagId) : IRequest<IEnumerable<Item>>;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class GetUncheckedItemsByTagQueryHandler : IRequestHandler<GetUncheckedItemsByTagQuery, IEnumerable<Item>>
{
    private readonly IDbConnection _connection;

    public GetUncheckedItemsByTagQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    // Handler method: Executes the query and returns the result
    public async Task<IEnumerable<Item>> Handle(GetUncheckedItemsByTagQuery request,
        CancellationToken cancellationToken)
    {
        // SQL query: Retrieves all unchecked items that have the specified tag
        // The query joins the Items, ItemTags, and Tags tables
        const string sql =
            @"SELECT i.Id, i.Name, i.WhenAdded, i.AddedBy, i.Checked, i.WhenChecked, t.Id AS TagId, t.TagName
                             FROM Items i
                             INNER JOIN ItemTags it ON i.Id = it.ItemId
                             INNER JOIN Tags t ON it.TagId = t.Id
                             WHERE it.TagId = @TagId AND i.Checked = FALSE";

        // Dictionary: Stores each item and its associated tags
        var itemDictionary = new Dictionary<int, Item>();

        // Executes the SQL query and maps the results to Item and Tag objects
        await _connection.QueryAsync<Item, Tag, Item>(sql,
            (item, tag) =>
            {
                // Checks if the current item is already in the dictionary
                if (!itemDictionary.TryGetValue(item.Id, out var currentItem))
                {
                    currentItem = item;
                    // Initializes an empty list of tags for the current item
                    currentItem.Tags = new List<Tag>();
                    // Adds the current item to the dictionary
                    itemDictionary.Add(currentItem.Id, currentItem);
                }

                // Adds the current tag to the list of tags for the current item
                ((List<Tag>)currentItem.Tags).Add(tag);
                return currentItem;
            },
            // Parameter: Specifies the tag ID for the WHERE clause in the SQL query
            new { request.TagId },
            // SplitOn: Specifies the column at which to split the result set into Item and Tag objects
            splitOn: "TagId");

        // Returns the list of items and their associated tags from the dictionary
        return itemDictionary.Values;
    }
}