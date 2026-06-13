using Kanban.Context;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/column")]
public class ColumnController(IColumnService columns) : ControllerBase
{
	[HttpPost]
	[EndpointSummary("Create a column by specifying a name.")]
	public ActionResult<ColumnResponse> Create(ColumnRequest request)
	{
		Column createdColumn = columns.Create(request.Name);

		return Created($"/api/column/{createdColumn.Id}", new ColumnResponse(
			createdColumn.Id,
			createdColumn.Name,
			createdColumn.Position
		));
	}

	[HttpDelete("{id}")]
	[EndpointSummary("Delete a column by ID.")]
	public ActionResult Delete(string id)
	{
		bool result = columns.Delete(id);

		return result
			? NoContent()
			: NotFound();
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a column and its ticket IDs, ordered by their position.")]
	public ActionResult<ColumnDetailResponse> Get(string id)
	{
		Column? column = columns.Get(id);
		if (column is null) return NotFound();

		return Ok(new ColumnDetailResponse(
			column.Id,
			column.Name,
			column.Position,
			[.. column.Tickets.OrderBy((x) => x.Position).Select((x) => x.Id)]
		));
	}

	[HttpGet]
	[EndpointSummary("Get al endpoints, ordered by their position.")]
	public ActionResult<List<ColumnResponse>> GetAllOrdered()
	{
		List<ColumnResponse> response = [.. columns
			.GetAllOrdered()
			.Select((x) => new ColumnResponse(x.Id, x.Name, x.Position))
		];

		return Ok(response);
	}
}