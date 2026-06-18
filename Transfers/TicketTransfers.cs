namespace Kanban.Transfers;

public record TicketCreateRequest(
	string Title,
	string? Description
);

public record TicketUpdateRequest(
	int? Position,
	string? Title,
	string? Description,
	string? ColumnId,
	string? ParentId,
	string? ChildId
);