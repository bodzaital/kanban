using System.Net;

namespace Kanban.Data;

public record ErrorBase(HttpStatusCode StatusCode);

public record ColumnNotFound() : ErrorBase(HttpStatusCode.NotFound);
public record ColumnHasTickets() : ErrorBase(HttpStatusCode.BadRequest);
public record TicketNotFound() : ErrorBase(HttpStatusCode.NotFound);