﻿namespace NextInLine.Server.CQRS.Queries.Models;

public class Tag
{
    public int Id { get; set; }
    public string TagName { get; set; } = default!;
}