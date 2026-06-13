using Kanban.Context;

namespace Kanban.Services;

public interface ITicketService
{
	Ticket? Create(string title, string description, string columnId);
	Ticket? Get(string id);
}

public class TicketService(KanbanContext context) : ITicketService
{
	public Ticket? Create(string title, string description, string columnId)
	{
		Column? column = context.Columns.Find(columnId);
		if (column is null) return null;
		
		context.Entry(column).Collection((x) => x.Tickets).Load();

		int? lastPosition = column.Tickets
			.OrderByDescending((x) => x.Position)
			.FirstOrDefault()?.Position;

		int? lastNumber = column.Tickets
			.OrderByDescending((x) => x.Number)
			.FirstOrDefault()?.Number;

		Ticket ticket = new()
		{
			Number = (lastNumber ?? 0) + 1,
			Position = (lastPosition ?? -1) + 1,
			Title = title,
			Description = description,
			Column = column,
		};

		context.Ticket.Add(ticket);
		context.SaveChanges();

		return ticket;
	}

	public Ticket? Get(string id)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return null;

		context.Entry(ticket).Reference((x) => x.Column).Load();

		return ticket;
	}
}