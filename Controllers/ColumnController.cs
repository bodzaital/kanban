using Kanban.Context;
using Kanban.Data;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/column")]
public class ColumnController(IColumnService columns, ITicketService tickets) : ControllerParent
{
	[HttpPost]
	[EndpointSummary("Create a column")]
	public ActionResult CreateColumn([FromBody] ColumnCreateRequest body)
	{
		OneOf<Column, ErrorBase> result = columns.Create(body.Name);

		return result.Match(
			(column) => Created($"/api/column/{column.Id}", column.ToSimpleResponse()),
			Error
		);
	}

	[HttpDelete("{id}")]
	[EndpointSummary("Delete a column")]
	public ActionResult DeleteColumn(string id)
	{
		OneOf<bool, ErrorBase> result = columns.Delete(id);

		return result.Match(
			(_) => NoContent(),
			Error
		);
	}

	[HttpPost("{id}/ticket")]
	[EndpointSummary("Create a ticket in a column")]
	public ActionResult CreateTicketInColumn(string id, [FromBody] TicketCreateRequest body)
	{
		OneOf<Ticket, ErrorBase> result = tickets.Create(body.Title, body.Description, id);

		return result.Match(
			(ticket) => Created($"/api/ticket/{ticket.Id}", ticket.ToSimpleResponse()),
			Error
		);
	}

	[HttpPatch("{id}")]
	[EndpointSummary("Update a column")]
	public ActionResult UpdateColumn(string id, [FromBody] CustomRequestCultureProvider body)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[EndpointSummary("Get columns ordered by position")]
	public ActionResult GetColumnsOrdered()
	{
		List<Column> result = columns.GetAllOrdered();

		return Ok(result.Select((column) => column.ToSimpleResponse()).ToList());
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a column")]
	public ActionResult GetColumn(string id)
	{
		OneOf<Column, ErrorBase> result = columns.Get(id);

		return result.Match(
			(column) => Ok(column.ToDetailResponse()),
			Error
		);
	}
}