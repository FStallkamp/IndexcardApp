using System.ComponentModel.DataAnnotations;

namespace IndexCardWebpage
{
	public class CardCategories
	{
		[Key]
		public int KategorieId { get; set; }
		public string Name { get; set; }
		public string Beschreibung { get; set; }
		public string Ersteller { get; set; }
	}
}
