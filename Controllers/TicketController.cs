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
public class TicketController(ITicketService tickets) : ControllerParent
{
	[HttpDelete("{id}")]
	[EndpointSummary("Delete a ticket")]
	public ActionResult DeleteTicket(string id, [FromQuery] bool cascade = false)
	{
		OneOf<bool, ErrorBase> result = tickets.Delete(id, cascade);

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
			(ticket) => Ok(ticket.ToDetailResponse()),
			Error
		);
	}

	[HttpPut("{id}/parent")]
	[EndpointSummary("Connect the parent of a ticket")]
	public ActionResult ConnectParentTicket(string id, [FromBody] TicketStructureRequest body)
	{
		OneOf<Ticket, ErrorBase> result = tickets.SetParent(id, body.Id);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse()),
			Error
		);
	}

	[HttpDelete("{id}/parent")]
	[EndpointSummary("Disconnect the parent of a ticket")]
	public ActionResult DisconnectParentTicket(string id)
	{
		OneOf<Ticket, ErrorBase> result = tickets.SetParent(id);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse()),
			Error
		);
	}

	[HttpPut("{id}/child")]
	[EndpointSummary("Connect the parent of a ticket")]
	public ActionResult ConnectChildTicket(string id, [FromBody] TicketStructureRequest body)
	{
		OneOf<Ticket, ErrorBase> result = tickets.SetChild(id, body.Id);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse()),
			Error
		);
	}

	[HttpDelete("{id}/child")]
	[EndpointSummary("Disconnect the parent of a ticket")]
	public ActionResult DisconnectChildTicket(string id, [FromBody] TicketStructureRequest body)
	{
		OneOf<Ticket, ErrorBase> result = tickets.SetChild(id, body.Id, true);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse()),
			Error
		);
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a ticket")]
	public ActionResult GetTicket(string id)
	{
		OneOf<Ticket, ErrorBase> result = tickets.Get(id);

		return result.Match(
			(ticket) => Ok(ticket.ToDetailResponse()),
			Error
		);
	}
}