namespace Kanban.Transfers;

public record TicketResponse(
	string Id,
	int Number,
	string Description
);