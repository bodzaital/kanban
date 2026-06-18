namespace Kanban.Transfers;

public record ColumnCreateRequest(
	string Name
);

public record ColumnUpdateRequest(
	string? Name,
	int? Position
);