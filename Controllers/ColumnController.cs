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
		if (request.Name is null) return BadRequest("The name must be specified.");
		if (request.Position is not null) return BadRequest("The position cannot be specified.");

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

	[HttpPatch("{id}")]
	[EndpointSummary("Update a column.")]
	public ActionResult<ColumnResponse> Update(string id, [FromBody] ColumnRequest request)
	{
		if (request.Position is not null)
		{
			bool reorderResult = columns.Reorder(id, request.Position.Value);
			if (!reorderResult) return NotFound();
		}

		if (request.Name is not null)
		{
			bool renameResult = columns.Rename(id, request.Name);
			if (!renameResult) return NotFound();
		}

		return Ok(columns.Get(id));
	}
}