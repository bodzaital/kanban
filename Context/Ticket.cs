namespace Kanban.Context;

public class Ticket
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required string Number { get; set; }
	public required string Description { get; set; }
	public required Column Column { get; set; }
}