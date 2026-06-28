namespace Kanban.Transfers;

public record ColumnCreateRequest(
	string Name
);

public record ColumnUpdateRequest(
	string? Name,
	int? Position
);

public record ColumnDetailResponse(
	string Id,
	string Name,
	int Position,
	List<string> Tickets
);