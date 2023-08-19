using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using MediatR;

namespace NextInLine.Server.CQRS.Commands;

public record AddTagCommand([Required] string TagName) : IRequest<AddTagResult>;

public record AddTagResult(bool Success, string Message, int? TagId = null);

public class AddTagCommandHandler : IRequestHandler<AddTagCommand, AddTagResult>
{
    private readonly IDbConnection _connection;

    public AddTagCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<AddTagResult> Handle(AddTagCommand request, CancellationToken cancellationToken)
    {
        var existingTagSql = @"SELECT Id FROM Tags WHERE LOWER(TagName) = LOWER(@TagName);";
        var existingTagId = await _connection.QueryFirstOrDefaultAsync<int?>(existingTagSql,
            new { request.TagName });

        if (existingTagId.HasValue) return new AddTagResult(true, "Tag already exists.", existingTagId);

        var insertTagSql = @"INSERT INTO Tags(TagName) VALUES (@TagName) RETURNING Id;";
        var newTagId = await _connection.QuerySingleAsync<int>(insertTagSql, new { request.TagName });

        return new AddTagResult(true, "Tag added successfully.", newTagId);
    }
}