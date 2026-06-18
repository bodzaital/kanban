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
		throw new NotImplementedException();
	}

	[HttpDelete("{id}")]
	[EndpointSummary("Delete a column")]
	public ActionResult DeleteColumn(string id)
	{
		throw new NotImplementedException();
	}

	[HttpPost("{id}/ticket")]
	[EndpointSummary("Create a ticket in a column")]
	public ActionResult CreateTicketInColumn(string id, [FromBody] TicketCreateRequest body)
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a column")]
	public ActionResult GetColumn(string id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("{id}/ticket")]
	[EndpointSummary("Get tickets ordered by position")]
	public ActionResult GetTicketsOrdered(string id)
	{
		throw new NotImplementedException();
	}
}