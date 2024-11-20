using System.ComponentModel.DataAnnotations;

namespace IndexCardWebpage
{
	public class IndexCard
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int KategorieId { get; set; }
	}
}
