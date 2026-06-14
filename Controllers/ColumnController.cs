using Kanban.Context;
using Kanban.Data;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/column")]
public class ColumnController(IColumnService columns) : ControllerParent
{
	[HttpPost]
	[EndpointSummary("Create a column by specifying a name.")]
	public ActionResult<ColumnResponse> Create(ColumnRequest request)
	{
		if (request.Name is null) return BadRequest("The name must be specified.");
		if (request.Position is not null) return BadRequest("The position cannot be specified.");

		Column createdColumn = columns.Create(request.Name);

		return Created($"/api/column/{createdColumn.Id}", createdColumn.ToResponse());
	}

	[HttpDelete("{id}")]
	[EndpointSummary("Delete a column by ID.")]
	public IActionResult Delete(string id)
	{
		OneOf<bool, ErrorBase> result = columns.Delete(id);

		return result.Match((x) => NoContent(), Error);
	}

	[HttpGet("{id}")]
	[EndpointSummary("Get a column and its ticket IDs, ordered by their position.")]
	public ActionResult<ColumnResponse> Get(string id)
	{
		OneOf<Column, ErrorBase> result = columns.Get(id);

		return result.Match((x) => Ok(x.ToResponse()), Error);
	}

	[HttpGet]
	[EndpointSummary("Get al endpoints, ordered by their position.")]
	public ActionResult<List<ColumnResponse>> GetAllOrdered()
	{
		List<ColumnResponse> response = [.. columns
			.GetAllOrdered()
			.Select((x) => x.ToResponse())
		];

		return Ok(response);
	}

	[HttpPatch("{id}/position")]
	[EndpointSummary("Update the position of a column")]
	public ActionResult<ColumnResponse> UpdatePosition(string id, [FromBody] ColumnPositionRequest request)
	{
		OneOf<Column, ErrorBase> result = columns.Reorder(id, request.Position);

		return result.Match(Ok, Error);

	}

	[HttpPatch("{id}/name")]
	[EndpointSummary("Update the name of a column")]
	public ActionResult<ColumnResponse> UpdateName(string id, [FromBody] ColumnNameRequest request)
	{
		OneOf<Column, ErrorBase> result = columns.Rename(id, request.Name);

		return result.Match(Ok, Error);
	}
}