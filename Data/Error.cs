using System.Net;

namespace Kanban.Data;

public record ErrorBase(HttpStatusCode StatusCode);

public record ColumnNotFound() : ErrorBase(HttpStatusCode.NotFound);
public record TicketNotFound() : ErrorBase(HttpStatusCode.NotFound);