using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexCardWebpage.Components.Pages
{
    public partial class Home
    {
        [Inject]
        CardCategoriesService CardCategoriesService { get; set; }

        public Home()
        {
        }

        private List<CardCategories> _categories;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Lade die Kategorien
                _categories = await CardCategoriesService.GetAllCategoriesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Kategorien: {ex.Message}");
                _categories = new List<CardCategories>(); // Leere Liste als Fallback
            }
        }
    }
}
