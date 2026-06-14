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
	OneOf<Column, ErrorBase> Reorder(string id, int newPosition);
	OneOf<Column, ErrorBase> Rename(string id, string newName);
}

public class ColumnService(KanbanContext context) : IColumnService
{
	public Column Create(string name)
	{
		int? lastPosition = context.Columns
			.OrderByDescending((x) => x.Position)
			.FirstOrDefault()?.Position;

		int nextPosition = (lastPosition ?? -1) + 1;

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
		
		context.Columns.Remove(column);
		RePositionColumns(column.Position);
		
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

	public OneOf<Column, ErrorBase> Reorder(string id, int newPosition)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return new ColumnNotFound();

		List<Column> columnsAfterExceptThat = [.. context.Columns
			.Where((x) => x.Position >= newPosition)
			.Where((x) => x.Id != id)
		];

		int nextPosition = newPosition + 1;

		columnsAfterExceptThat.ForEach((x) => x.Position = nextPosition++);
		column.Position = newPosition;

		context.SaveChanges();

		return column;
	}

	public OneOf<Column, ErrorBase> Rename(string id, string newName)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return new ColumnNotFound();

		column.Name = newName;
		context.SaveChanges();
		
		return column;
	}

	private void RePositionColumns(int position)
	{
		List<Column> columnsAfter = [.. context.Columns.Where((x) => x.Position > position)];
		columnsAfter.ForEach((x) => x.Position = position++);
	}
}