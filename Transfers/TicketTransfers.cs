namespace Kanban.Transfers;

public record TicketCreateRequest(
	string Title
);

public record TicketUpdateRequest(
	int? Position,
	string? Title,
	string? Description,
	string? ColumnId,
	string? ParentId,
	string? ChildId
);