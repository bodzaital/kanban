using Kanban.Transfers;

namespace Kanban.Context;

public class Column
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required string Name { get; set; }
	public required int Position { get; set; }
	public List<Ticket> Tickets { get; set; } = [];

	public ColumnDetailResponse ToDetailResponse(string prefix) => new(
		Id,
		Name,
		Position,
		[.. Tickets.OrderBy((x) => x.Position).Select((x) => x.ToDetailResponse(prefix))]
	);
}