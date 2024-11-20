using Microsoft.EntityFrameworkCore;

namespace IndexCardWebpage
{
	public class CardContextFactory
	{
		public CardContext CreateDbContext(string[] args)
		{
			var path = Path.Combine(AppContext.BaseDirectory, "data.db");
			var connectionstring = $"Data Source={path}";

			var builder = new DbContextOptionsBuilder<CardContext>();
			builder.UseSqlite(connectionstring);
			var context = new CardContext(builder.Options);
			context.Database.EnsureCreated();
			return context;
		}
	}
}
