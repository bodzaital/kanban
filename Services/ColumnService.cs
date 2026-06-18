using Kanban.Context;
using Kanban.Data;
using OneOf;

namespace Kanban.Services;

public interface IColumnService
{
	Column Create(string name);
	OneOf<bool, ErrorBase> Delete(string id);
	List<Column> GetAllOrdered();
	OneOf<Column, ErrorBase> Get(string id);
	OneOf<Column, ErrorBase> Update(string id, string? name, int? position);
}

public class ColumnService(KanbanContext context) : IColumnService
{
	public Column Create(string name)
	{
		int nextPosition = GetLastPosition() + 1;

		Column column = new()
		{
			Name = name,
			Position = nextPosition,
		};

		context.Columns.Add(column);
		context.SaveChanges();

		return column;
	}

	public OneOf<bool, ErrorBase> Delete(string id)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return new ColumnNotFound();

		context.Entry(column).Collection((x) => x.Tickets).Load();

		if (column.Tickets.Count > 0) return new ColumnHasTickets();
		
		context.Columns.Remove(column);
		ShiftColumnsLeftByOne(column.Position);
		
		context.SaveChanges();
		return true;
	}

	public List<Column> GetAllOrdered()
	{
		return [.. context.Columns.OrderBy((x) => x.Position)];
	}

	public OneOf<Column, ErrorBase> Get(string id)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return new ColumnNotFound();

		context.Entry(column).Collection((x) => x.Tickets).Load();

		return column;
	}

	public OneOf<Column, ErrorBase> Update(string id, string? name, int? position)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return new ColumnNotFound();

		column.Name = name ?? column.Name;
		
		if (position is not null) ReorderColumns(column, position.Value);

		context.SaveChanges();
		return column;
	}

	private void ShiftColumnsLeftByOne(int nextPosition)
	{
		context.Columns
			.Where((x) => x.Position > nextPosition)
			.OrderBy((x) => x.Position)
			.ToList().ForEach((x) => x.Position = nextPosition++);
	}

	private int GetLastPosition()
	{
		bool hasColumns = context.Columns.Any();

		if (!hasColumns) return -1;

		int lastColumnPosition = context.Columns
			.OrderBy((x) => x.Position)
			.Last()
			.Position;
		
		return lastColumnPosition;
	}

	private void ReorderColumns(Column column, int newPosition)
	{
		int oldPosition = column.Position;

		if (oldPosition == newPosition) return;

		if (oldPosition > newPosition) context.Columns
			.Where((x) => x.Id != column.Id)
			.Where((x) => x.Position <= oldPosition && x.Position >= newPosition)
			.OrderBy((x) => x.Position).ToList()
			.ForEach((x) => x.Position += 1);

		if (oldPosition < newPosition) context.Columns
			.Where((x) => x.Id != column.Id)
			.Where((x) => x.Position >= oldPosition && x.Position <= newPosition)
			.OrderBy((x) => x.Position).ToList()
			.ForEach((x) => x.Position -= 1);

		column.Position = newPosition;
	}
}