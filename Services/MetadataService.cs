using Kanban.Context;

namespace Kanban.Services;

public interface IMetadataService
{
	public int LastNumber { get; set; }
}

public class MetadataService(KanbanContext context, IConfiguration configuration) : IMetadataService
{
	public int LastNumber
	{
		get
		{
			EnsureInDatabase<int>(nameof(LastNumber));

			return int.Parse(context.Metadata.Find(nameof(LastNumber))!.Value);
		}

		set
		{
			EnsureInDatabase<int>(nameof(LastNumber));

			context.Metadata.Find(nameof(LastNumber))!.Value = value.ToString();

			context.SaveChanges();
		}
	}

	private void EnsureInDatabase<T>(string id)
	{
		bool isInDatabase = context.Metadata.Find(id) is not null;
		if (isInDatabase) return;

		string value = configuration
			.GetSection("RequiredProperties")
			.GetValue<T>(nameof(LastNumber))!
			.ToString()!;

		Metadata metadata = new()
		{
			Id = id,
			Value = value,
		};

		context.Metadata.Add(metadata);
		context.SaveChanges();
	}
}