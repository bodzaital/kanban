using System.Diagnostics.CodeAnalysis;
using Kanban.Context;
using Kanban.Data;
using Kanban.Services;
using Kanban.Transfers;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace Kanban.Controllers;

[ApiController]
[Route("/api/column")]
public class TicketController(ITicketService tickets) : ControllerParent
{
	
}