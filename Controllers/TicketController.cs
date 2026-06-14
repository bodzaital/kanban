using System.Diagnostics.CodeAnalysis;
using Kanban.Context;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/column/{columnId}")]
public class TicketController(ITicketService tickets) : ControllerBase
{
	[HttpPost("ticket")]
	[EndpointSummary("Create a ticket.")]
	public ActionResult<TicketResponse> Create(string columnId, [FromBody] TicketRequest request)
	{
		Ticket? ticket = tickets.Create(
			request.Title,
			request.Description,
			columnId
		);

		return ticket is null
			? NotFound("Column not found.")
			: Created($"/api/column/{columnId}/ticket/{ticket.Id}", new TicketResponse(
				ticket.Id,
				ticket.Number,
				ticket.Title,
				ticket.Description,
				ticket.Column.Id
			));
	}

	[HttpGet("ticket/{id}")]
	[EndpointSummary("Get a ticket by ID.")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Swagger UI requires it")]
	public ActionResult<TicketResponse> Get(string columnId, string id)
	{
		Ticket? ticket = tickets.Get(id);
		if (ticket is null) return NotFound();

		return Ok(new TicketResponse(
			ticket.Id,
			ticket.Number,
			ticket.Title,
			ticket.Description,
			ticket.Column.Id
		));
	}

	[HttpDelete("ticket/{id}")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Swagger UI requires it")]
	public ActionResult Delete(string columnId, string id)
	{
		bool result = tickets.Delete(id);

		return result
			? NoContent()
			: NotFound();
	}
}