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
		if (request.Title is null || request.Description is null) return BadRequest("The title or description must be specified.");
		if (request.Position is not null || request.ColumnId is not null) return BadRequest("The position or columnd ID may not be specified.");

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

	[HttpPatch("ticket/{id}")]
	[EndpointSummary("Update a ticket")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Swagger UI requires it")]
	public ActionResult<TicketResponse> Update(string columnId, string id, [FromBody] TicketRequest request)
	{
		if (request.Title is not null)
		{
			bool retitleResult = tickets.Retitle(id, request.Title);
			if (!retitleResult) return NotFound();
		}

		if (request.Description is not null)
		{
			bool updateDescriptionResult = tickets.UpdateDescription(id, request.Description);
			if (!updateDescriptionResult) return NotFound();
		}

		if (request.Position is not null)
		{
			bool repositionResult = tickets.Reorder(id, request.Position.Value);
			if (!repositionResult) return NotFound();
		}

		if (request.ColumnId is not null)
		{
			bool moveResult = tickets.MoveToColumn(id, request.ColumnId);
			if (!moveResult) return NotFound();
		}

		return Ok(tickets.Get(id));
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