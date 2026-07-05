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
	OneOf<bool, ErrorBase> Delete(string id);
	OneOf<Ticket, ErrorBase> Update(string id, int? position, string? title, string? description, string? columnId);
	OneOf<Ticket, ErrorBase> MoveColumn(string id, string? columnId);
	OneOf<Ticket, ErrorBase> GetByNumber(string number, string prefix);
}

public class TicketService(KanbanContext context, IMetadataService metadata) : ITicketService
{
	public OneOf<Ticket, ErrorBase> Create(string title, string? description, string columnId)
	{
		Column? column = context.Columns.Find(columnId);
		if (column is null) return new ColumnNotFound();
		
		context.Entry(column).Collection((x) => x.Tickets).Load();

		int nextPosition = GetLastPosition(column) + 1;
		int nextNumber = ++metadata.LastNumber;

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

	public OneOf<Ticket, ErrorBase> Update(string id, int? position, string? title, string? description, string? columnId)
	{
		Ticket? ticket = context.Tickets.Find(id);
		if (ticket is null) return new TicketNotFound();

		context.Entry(ticket).Reference((x) => x.Column).Load();

		ticket.Title = title ?? ticket.Title;
		ticket.Description = description ?? ticket.Description;

		if (columnId is not null)
		{
			OneOf<Ticket, ErrorBase> result = MoveColumn(id, columnId);
			if (result.IsT1) return result.AsT1;
		}

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

	public OneOf<Ticket, ErrorBase> GetByNumber(string number, string prefix)
	{
		number = number.StartsWith(prefix)
			? number[(prefix.Length + 1)..]
			: number;

		if (!int.TryParse(number, out int ticketNumber))
		{
			return new TicketPrefixInvalid();
		}

		string? id = context.Tickets
			.Where((x) => x.Number == ticketNumber)
			.Select((x) => x.Id)
			.FirstOrDefault();
		
		if (id is null) return new TicketNotFound();

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
}