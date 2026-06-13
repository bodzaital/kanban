using Microsoft.EntityFrameworkCore;

namespace Kanban.Context;

public class KanbanContext(DbContextOptions<KanbanContext> ctx) : DbContext(ctx)
{
	public DbSet<Column> Columns { get; set; }

	public DbSet<Ticket> Ticket { get; set; }
}