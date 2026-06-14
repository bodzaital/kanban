namespace Kanban.Transfers;

public record TicketRequest(
	string? Title,
	string? Description,
	int? Position,
	string? ColumnId
);

public record TicketResponse(
	string Id,
	int Number,
	int Position,
	string Title,
	string Description,
	string ColumnId
);