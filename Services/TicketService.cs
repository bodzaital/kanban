using System.IO.Pipes;
using Kanban.Context;
using Kanban.Data;
using OneOf;
using SQLitePCL;

namespace Kanban.Services;

public interface ITicketService
{
	OneOf<Ticket, ErrorBase> Create(string title, string? description, string columnId);
	OneOf<Ticket, ErrorBase> Get(string id);
	OneOf<bool, ErrorBase> Delete(string id, bool cascade);
	OneOf<Ticket, ErrorBase> Update(string id, int? position, string? title, string? description);
	OneOf<Ticket, ErrorBase> MoveColumn(string id, string? columnId);
	OneOf<Ticket, ErrorBase> SetParent(string id, string? parentId = null);
	OneOf<Ticket, ErrorBase> SetChild(string id, string childId, bool delete = false);
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

		return Get(ticket.Id);
	}

	public OneOf<Ticket, ErrorBase> Get(string id)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();
		context.Entry(ticket).Reference((x) => x.Parent).Load();
		context.Entry(ticket).Collection((x) => x.Children).Load();

		return ticket;
	}

	public OneOf<bool, ErrorBase> Delete(string id, bool cascade)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();
		context.Entry(ticket).Collection((x) => x.Children).Load();

		if (cascade) for (int i = 0; i < ticket.Children.Count; i++)
		{
			Delete(ticket.Children[i].Id, cascade);
		}
		
		ticket.Parent = null;
		context.Tickets.Remove(ticket);

		ShiftTicketsUpByOne(ticket.Column, ticket.Position);

		context.SaveChanges();
		return true;
	}

	public OneOf<Ticket, ErrorBase> Update(string id, int? position, string? title, string? description)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();

		ticket.Title = title ?? ticket.Title;
		ticket.Description = description ?? ticket.Description;

		if (position is not null) ReorderTickets(ticket, position.Value);

		context.SaveChanges();
		return Get(id);
	}

	public OneOf<Ticket, ErrorBase> MoveColumn(string id, string? columnId)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		Column? newColumn = context.Columns.Find(columnId);
		if (newColumn is null) return new ColumnNotFound();

		Column originalColumn = ticket.Column;
		ticket.Column = newColumn;

		ShiftTicketsUpByOne(originalColumn, ticket.Position);
		ticket.Position = GetLastPosition(newColumn) + 1;

		context.SaveChanges();
		return Get(id);
	}

	public OneOf<Ticket, ErrorBase> SetParent(string id, string? parentId = null)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Parent).Load();

		if (parentId is null)
		{
			ticket.Parent = null;
			context.SaveChanges();
			
			return Get(id);
		}

		Ticket? parentTicket = context.Tickets.Find(parentId);
		if (parentTicket is null) return new ParentTicketNotFound();

		ticket.Parent = parentTicket;
		context.SaveChanges();

		return Get(id);
	}

	public OneOf<Ticket, ErrorBase> SetChild(string id, string childId, bool delete = false)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Collection((x) => x.Children).Load();

		Ticket? childTicket = context.Tickets.Find(childId);
		if (childTicket is null) return new ChildTicketNotFound();

		if (!delete)
		{
			ticket.Children.Add(childTicket);
			context.SaveChanges();

			return Get(id);
		}

		bool isParentOfChild = ticket.Children.Contains(childTicket);
		if (!isParentOfChild) return new TicketNotParentOfChild();

		ticket.Children.Remove(childTicket);
		context.SaveChanges();

		return Get(id);
	}

	private void ReorderTickets(Ticket ticket, int newPosition)
	{
		int oldPosition = ticket.Position;

		if (oldPosition == newPosition) return;

		context.Entry(ticket.Column).Collection((x) => x.Tickets).Load();

		if (oldPosition > newPosition) ticket.Column.Tickets
			.Where((x) => x.Id != ticket.Id)
			.Where((x) => x.Position <= oldPosition && x.Position >= newPosition)
			.OrderBy((x) => x.Position).ToList()
			.ForEach((x) => x.Position += 1);

		if (oldPosition < newPosition) ticket.Column.Tickets
			.Where((x) => x.Id != ticket.Id)
			.Where((x) => x.Position >= oldPosition && x.Position <= newPosition)
			.OrderBy((x) => x.Position).ToList()
			.ForEach((x) => x.Position -= 1);

		ticket.Position = newPosition;
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