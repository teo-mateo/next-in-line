namespace NextInLine.Server.CQRS.Queries.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime WhenAdded { get; set; }
    public string AddedBy { get; set; }
    public bool Checked { get; set; }
    public DateTime? WhenChecked { get; set; }
    public IEnumerable<Tag> Tags { get; set; }
}