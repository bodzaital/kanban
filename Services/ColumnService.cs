using Kanban.Context;

namespace Kanban.Services;

public interface IColumnService
{
	Column Create(string name);
	bool Delete(string id);
	List<Column> GetAllOrdered();
	Column? Get(string id);
	bool Reorder(string id, int newPosition);
	bool Rename(string id, string newName);
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

	public bool Delete(string id)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return false;
		
		context.Columns.Remove(column);
		RePositionColumns(column.Position);
		
		context.SaveChanges();
		return true;
	}

	public List<Column> GetAllOrdered()
	{
		return [.. context.Columns.OrderBy((x) => x.Position)];
	}

	public Column? Get(string id)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return null;

		context.Entry(column).Collection((x) => x.Tickets).Load();

		return column;
	}

	public bool Reorder(string id, int newPosition)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return false;

		List<Column> columnsAfterExceptThat = [.. context.Columns
			.Where((x) => x.Position >= newPosition)
			.Where((x) => x.Id != id)
		];

		int nextPosition = newPosition + 1;

		columnsAfterExceptThat.ForEach((x) => x.Position = nextPosition++);
		column.Position = newPosition;

		context.SaveChanges();

		return true;
	}

	public bool Rename(string id, string newName)
	{
		Column? column = context.Columns.Find(id);
		if (column is null) return false;

		column.Name = newName;
		context.SaveChanges();
		
		return true;
	}

	private void RePositionColumns(int position)
	{
		List<Column> columnsAfter = [.. context.Columns.Where((x) => x.Position > position)];
		columnsAfter.ForEach((x) => x.Position = position++);
	}
}