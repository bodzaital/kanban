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
	public ActionResult DeleteTicket(string id)
	{
		throw new NotImplementedException();
	}

	[HttpPatch("{id}")]
	[EndpointSummary("Update a ticket")]
	public ActionResult UpdateTicket(string id, [FromBody] TicketUpdateRequest body)
	{
		throw new NotImplementedException();
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a ticket")]
	public ActionResult GetTicket(string id)
	{
		throw new NotImplementedException();
	}
}