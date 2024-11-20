using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IndexCardWebpage
{
	public class CardContext : DbContext
	{
		public CardContext(DbContextOptions<CardContext> options) : base(options)
		{
		}

		// Aufsetzung der Datenbankstruktur mithilfe der Klasse
		public DbSet<IndexCard> IndexCards { get; set; }
		public DbSet<CardCategories> CardCategories { get; set; }
		public DbSet<CategoryCreator> CategoryCreator { get; set; }
	}
}
