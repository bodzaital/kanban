using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Context;

public class KanbanContext(DbContextOptions<KanbanContext> ctx) : DbContext(ctx)
{
	public DbSet<Column> Columns { get; set; }

	public DbSet<Ticket> Tickets { get; set; }

	public DbSet<Metadata> Metadata { get; set; }
}