using System.Diagnostics.CodeAnalysis;
using Kanban.Context;
using Kanban.Data;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/column/{columnId}")]
public class TicketController(ITicketService tickets) : ControllerParent
{
	[HttpPost("ticket")]
	[EndpointSummary("Create a ticket.")]
	public ActionResult<TicketResponse> Create(string columnId, [FromBody] TicketRequest request)
	{
		if (request.Title is null || request.Description is null) return BadRequest("The title or description must be specified.");
		if (request.Position is not null || request.ColumnId is not null) return BadRequest("The position or columnd ID may not be specified.");

		OneOf<Ticket, ErrorBase> ticket = tickets.Create(
			request.Title,
			request.Description,
			columnId
		);

		string url = $"/api/column/{columnId}/ticket/{ticket.AsT0.Id}";
		return ticket.Match((x) => Created(url, x.ToResponse()), Error);
	}

	[HttpGet("ticket/{id}")]
	[EndpointSummary("Get a ticket by ID.")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Swagger UI requires it")]
	public ActionResult<TicketResponse> Get(string columnId, string id)
	{
		OneOf<Ticket, ErrorBase> result = tickets.Get(id);

		return result.Match(Ok, Error);
	}

	// [HttpPatch("ticket/{id}")]
	// [EndpointSummary("Update a ticket")]
	// [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Swagger UI requires it")]
	// public ActionResult<TicketResponse> Update(string columnId, string id, [FromBody] TicketRequest request)
	// {
	// 	// TODO: WIP
	// 	if (request.Title is not null)
	// 	{
	// 		bool retitleResult = tickets.Retitle(id, request.Title);
	// 		if (!retitleResult) return NotFound();
	// 	}

	// 	if (request.Description is not null)
	// 	{
	// 		bool updateDescriptionResult = tickets.UpdateDescription(id, request.Description);
	// 		if (!updateDescriptionResult) return NotFound();
	// 	}

	// 	if (request.Position is not null)
	// 	{
	// 		bool repositionResult = tickets.Reorder(id, request.Position.Value);
	// 		if (!repositionResult) return NotFound();
	// 	}

	// 	Ticket ticket = tickets.Get(id)!;

	// 	return Ok(ticket.ToResponse());
	// }

	// [HttpPatch("ticket/{id}/column")]
	// public ActionResult<TicketResponse> MoveToColumn(string columnId, string id, [FromBody] TicketRequest request)
	// {
	// 	// TODO: WIP
	// 	if (request.ColumnId is not null)
	// 	{
	// 		bool moveResult = tickets.MoveToColumn(id, request.ColumnId);
	// 		if (!moveResult) return NotFound();
	// 	}

	// 	Ticket ticket = tickets.Get(id)!;

	// 	return Ok(ticket.ToResponse());
	// }

	[HttpDelete("ticket/{id}")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Swagger UI requires it")]
	public ActionResult Delete(string columnId, string id)
	{
		OneOf<bool, ErrorBase> result = tickets.Delete(id);

		return result.Match((x) => NoContent(), Error);
	}
}