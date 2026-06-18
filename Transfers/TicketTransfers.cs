namespace Kanban.Transfers;

public record TicketCreateRequest(
	string Title,
	string? Description
);

public record TicketUpdateRequest(
	int? Position,
	string? Title,
	string? Description,
	string? ColumnId
);

public record TicketStructureRequest(
	string Id
);

public record TicketSimpleResponse(
	string Id,
	int Position,
	string Title,
	int Number
);

public record TicketDetailResponse(
	string Id,
	int Number,
	int Position,
	string Title,
	string Description,
	string ColumnId,
	string? ParentId,
	List<string> ChildIds
);