using Kanban.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers;

public abstract class ControllerParent : ControllerBase
{
	protected ActionResult Error(ErrorBase value)
	{
		return StatusCode((int)value.StatusCode, value.GetType());
	}
}