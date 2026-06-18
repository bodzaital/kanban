using Kanban.Context;
using Kanban.Data;
using OneOf;

namespace Kanban.Services;

public interface ITicketService
{
	OneOf<Ticket, ErrorBase> Create(string title, string? description, string columnId);
	OneOf<Ticket, ErrorBase> Get(string id);
	OneOf<bool, ErrorBase> Delete(string id);
	OneOf<bool, ErrorBase> Reorder(string id, int newPosition);
	OneOf<bool, ErrorBase> Retitle(string id, string newTitle);
	OneOf<bool, ErrorBase> UpdateDescription(string id, string newDescription);
	OneOf<bool, ErrorBase> MoveToColumn(string id, string columnId);
	List<Ticket> GetAllOrdered(string columnId);
}

public class TicketService(KanbanContext context) : ITicketService
{
	public OneOf<Ticket, ErrorBase> Create(string title, string? description, string columnId)
	{
		Column? column = context.Columns.Find(columnId);
		if (column is null) return new ColumnNotFound();

		description ??= "";
		
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

	public OneOf<Ticket, ErrorBase> Get(string id)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();

		return ticket;
	}

	public OneOf<bool, ErrorBase> Delete(string id)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();

		context.Ticket.Remove(ticket);
		RePositionTickets(ticket.Column.Id, ticket.Position);

		context.SaveChanges();
		return true;
	}

	public OneOf<bool, ErrorBase> Reorder(string id, int newPosition)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return new TicketNotFound();

		List<Ticket> ticketsAfterExceptThat = [.. context.Ticket
			.Where((x) => x.Position >= newPosition)
			.Where((x) => x.Id != id)
		];

		int nextPosition = newPosition + 1;

		ticketsAfterExceptThat.ForEach((x) => x.Position = nextPosition++);
		ticket.Position = newPosition + 1;

		context.SaveChanges();

		return true;
	}

	public OneOf<bool, ErrorBase> Retitle(string id, string newTitle)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return new TicketNotFound();

		ticket.Title = newTitle;

		context.SaveChanges();

		return true;
	}

	public OneOf<bool, ErrorBase> UpdateDescription(string id, string newDescription)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return new TicketNotFound();

		ticket.Description = newDescription;

		context.SaveChanges();

		return true;
	}

	public OneOf<bool, ErrorBase> MoveToColumn(string id, string columnId)
	{
		Ticket? ticket = context.Ticket.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();
		string originalColumnId = ticket.Column.Id;

		Column? column = context.Columns.Find(columnId);
		if (column is null) return new ColumnNotFound();

		context.Entry(column).Collection((x) => x.Tickets).Load();
		int lastPositionInNewColumn = column.Tickets
			.OrderByDescending((x) => x.Position)
			.FirstOrDefault()?.Position ?? -1;

		ticket.Column = column;
		RePositionTickets(originalColumnId, ticket.Position);

		ticket.Position = lastPositionInNewColumn + 1;

		context.SaveChanges();

		return true;
	}

	public List<Ticket> GetAllOrdered(string columnId)
	{
		throw new NotImplementedException();
	}

	private void RePositionTickets(string columnId, int position)
	{
		Column column = context.Columns.Find(columnId)
			?? throw new Exception($"Expected column {columnId} to exist but it did not, failed to reposition tickets after ticket was deleted.");
		
		context.Entry(column).Collection((x) => x.Tickets).Load();

		List<Ticket> ticketsAfter = [.. column.Tickets.Where((x) => x.Position > position)];
		ticketsAfter.ForEach((x) => x.Position = position++);
	}
}