using System.ComponentModel.DataAnnotations;

namespace NextInLine.Server.Controllers.Models;

public class ModifyItemModel
{
    [Required] public string NewName { get; set; } = default!;
    public IEnumerable<int>? NewTagIds { get; set; }
}