using System.ComponentModel.DataAnnotations;

namespace IndexCardWebpage
{
	public class CategoryCreator
	{
		[Key]
		public int KategorieId { get; set; }
		public string Ersteller { get; set; }
	}
}
