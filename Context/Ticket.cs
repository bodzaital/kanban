using Kanban.Transfers;

namespace Kanban.Context;

public class Ticket
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required int Number { get; set; }
	public required int Position { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public required Column Column { get; set; }
	public Ticket? Parent { get; set; } = null;
	public List<Ticket> Children { get; set; } = [];

	public TicketSimpleResponse ToSimpleResponse() => new(
		Id,
		Position,
		Title,
		Number
	);

	public TicketDetailResponse ToDetailResponse() => new(
		Id,
		Number,
		Position,
		Title,
		Description,
		Column.Id,
		Parent?.Id,
		[.. Children.Select((x) => x.Id)]
	);
}