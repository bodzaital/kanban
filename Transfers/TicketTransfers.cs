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
	string Number
);

public record TicketDetailResponse(
	string Id,
	string Number,
	int Position,
	string Title,
	string Description,
	string ColumnId
);