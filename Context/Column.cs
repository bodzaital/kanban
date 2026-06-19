using Kanban.Transfers;

namespace Kanban.Context;

public class Column
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required string Name { get; set; }
	public required int Position { get; set; }
	public List<Ticket> Tickets { get; set; } = [];

	public ColumnSimpleResponse ToSimpleResponse() => new(
		Id,
		Name,
		Position
	);

	public ColumnDetailResponse ToDetailResponse() => new(
		Id,
		Name,
		Position,
		[.. Tickets.OrderBy((x) => x.Position).Select((x) => x.ToSimpleResponse())]
	);
}