using System.Net;

namespace Kanban.Data;

public record TicketNotFound() : ErrorBase(HttpStatusCode.NotFound);
public record ParentTicketNotFound() : ErrorBase(HttpStatusCode.BadRequest);
public record ChildTicketNotFound() : ErrorBase(HttpStatusCode.BadRequest);
public record TicketNotParentOfChild() : ErrorBase(HttpStatusCode.BadRequest);
public record TicketHasChildren() : ErrorBase(HttpStatusCode.BadRequest);
public record TicketPrefixInvalid() : ErrorBase(HttpStatusCode.BadRequest);