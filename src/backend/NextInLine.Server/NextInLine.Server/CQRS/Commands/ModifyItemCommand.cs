using System.Data;
using Dapper;
using MediatR;

namespace NextInLine.Server.CQRS.Commands;

public record ModifyItemCommand(int ItemId, string NewName, IEnumerable<int>? NewTagIds = null) : IRequest;

public class ModifyItemCommandHandler : IRequestHandler<ModifyItemCommand>
{
    private readonly IDbConnection _dbConnection;

    public ModifyItemCommandHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task Handle(ModifyItemCommand request, CancellationToken cancellationToken)
    {
        // Update the item's name
        var updateItemQuery = "UPDATE Items SET Name = @NewName WHERE Id = @ItemId";
        await _dbConnection.ExecuteAsync(updateItemQuery, new { request.NewName, request.ItemId });

        // Optionally update tags
        if (request.NewTagIds != null)
        {
            // Remove existing tags for the item
            var deleteTagsQuery = "DELETE FROM ItemTags WHERE ItemId = @ItemId";
            await _dbConnection.ExecuteAsync(deleteTagsQuery, new { request.ItemId });

            // Add the new tags for the item
            var insertTagsQuery = "INSERT INTO ItemTags (ItemId, TagId) VALUES (@ItemId, @TagId)";
            foreach (var tagId in request.NewTagIds)
                await _dbConnection.ExecuteAsync(insertTagsQuery, new { request.ItemId, TagId = tagId });
        }
    }
}