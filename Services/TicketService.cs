using Kanban.Context;
using Kanban.Data;
using OneOf;

namespace Kanban.Services;

public interface ITicketService
{
	OneOf<Ticket, ErrorBase> Create(string title, string? description, string columnId);
	OneOf<Ticket, ErrorBase> Get(string id);
	OneOf<bool, ErrorBase> Delete(string id);
}

public class TicketService(KanbanContext context) : ITicketService
{
	public OneOf<Ticket, ErrorBase> Create(string title, string? description, string columnId)
	{
		Column? column = context.Columns.Find(columnId);
		if (column is null) return new ColumnNotFound();
		
		context.Entry(column).Collection((x) => x.Tickets).Load();

		int nextPosition = GetLastPosition(column) + 1;
		int nextNumber = GetLastNumber() + 1;

		Ticket ticket = new()
		{
			Number = nextNumber,
			Position = nextPosition,
			Title = title,
			Description = description ?? "",
			Column = column,
		};

		context.Tickets.Add(ticket);
		context.SaveChanges();

		return ticket;
	}

	public OneOf<Ticket, ErrorBase> Get(string id)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();

		return ticket;
	}

	public OneOf<bool, ErrorBase> Delete(string id)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();

		context.Tickets.Remove(ticket);

		ShiftTicketsUpByOne(ticket.Column, ticket.Position);

		context.SaveChanges();
		return true;
	}

	private void ShiftTicketsUpByOne(Column column, int nextPosition)
	{
		context.Entry(column).Collection((x) => x.Tickets).Load();

		column.Tickets
			.Where((x) => x.Position > nextPosition)
			.OrderBy((x) => x.Position)
			.ToList().ForEach((x) => x.Position = nextPosition++);
	}

	private int GetLastPosition(Column column)
	{
		bool hasTickets = column.Tickets.Count > 0;
		if (!hasTickets) return -1;

		int lastTicketPosition = column.Tickets
			.OrderBy((x) => x.Position)
			.Last()
			.Position;

		return lastTicketPosition;
	}

	private int GetLastNumber()
	{
		bool hasTickets = context.Tickets.Any();
		if (!hasTickets) return -1;

		int lastTicketNumber = context.Tickets
			.OrderBy((x) => x.Number)
			.Last()
			.Number;

		return lastTicketNumber;
	}
}