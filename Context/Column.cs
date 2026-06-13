namespace Kanban.Context;

public class Column
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required string Name { get; set; }
	public required int Position { get; set; }
	public ICollection<Ticket> Tickets { get; set; } = [];
}