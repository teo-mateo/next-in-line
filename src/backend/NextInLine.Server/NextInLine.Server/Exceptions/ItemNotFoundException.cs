namespace NextInLine.Server.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(int itemId)
        : base($"Item with Id {itemId} was not found.")
    {
    }
}
