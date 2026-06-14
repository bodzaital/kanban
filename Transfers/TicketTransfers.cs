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
	string Title,
	string Description,
	string ColumnId
);