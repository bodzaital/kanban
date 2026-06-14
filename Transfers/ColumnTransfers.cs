namespace Kanban.Transfers;

public record ColumnRequest(
	string? Name,
	int? Position
);

public record ColumnResponse(
	string Id,
	string Name,
	int Position
);

public record ColumnDetailResponse(
	string Id,
	string Name,
	int Position,
	List<string> TicketIds
);