using System.Diagnostics.CodeAnalysis;
using Kanban.Context;
using Kanban.Data;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/ticket")]
public class TicketController(ITicketService tickets, IConfiguration configuration) : ControllerParent
{
	private readonly string _ticketPrefix = configuration.GetValue<string>("TicketPrefix")!;

	[HttpDelete("{id}")]
	[EndpointSummary("Delete a ticket")]
	public ActionResult DeleteTicket(string id)
	{
		OneOf<bool, ErrorBase> result = tickets.Delete(id);

		return result.Match(
			(_) => NoContent(),
			Error
		);
	}

	[HttpPatch("{id}")]
	[EndpointSummary("Update a ticket")]
	public ActionResult UpdateTicket(string id, [FromBody] TicketUpdateRequest body)
	{
		OneOf<Ticket, ErrorBase> result = tickets.Update(id, body.Position, body.Title, body.Description);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse(_ticketPrefix)),
			Error
		);
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a ticket")]
	public ActionResult GetTicket(string id)
	{
		OneOf<Ticket, ErrorBase> result = tickets.Get(id);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse(_ticketPrefix)),
			Error
		);
	}
}