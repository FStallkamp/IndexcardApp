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
        [Inject] CardCategoriesService CardCategoriesService { get; set; }
        [Inject] IndexCardService IndexCardService { get; set; } 

        private List<IndexCard> _cards = new List<IndexCard>();
        private List<IndexCard> DeletableCards = new List<IndexCard>();
        private CardCategories DeletableObject = new CardCategories();
        private DateTime _lastDeleteAttempt = DateTime.MinValue;
        private bool TimeError = false;
        private int DeleteTimer = 10;
        private bool DeleteCategoryBool = false;
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

        private void NavigateToRandomLearnPage()
        {
            NavigationManager.NavigateTo("/learn/all");
        }

        private void EditCategory(CardCategories category)
        {
            NavigationManager.NavigateTo($"/edit/{category.Name}");
        }

        private async Task DeleteCategory(CardCategories categorie)
        {

            if ((DateTime.Now - _lastDeleteAttempt).TotalSeconds < DeleteTimer)
            {
                TimeError = true;
                StateHasChanged(); // UI aktualisieren
                return;
            }

            _lastDeleteAttempt = DateTime.Now;

            DeletableObject = categorie;
            try
            {
                foreach (var card in _cards)
                {
                    if (DeletableObject.KategorieId == card.KategorieId)
                    {
                        DeletableCards.Add(card);
                    }
                }
                DeleteCategoryBool = true;
            }
            catch
            {
                Console.WriteLine("Kategorie nicht gefunden!");
            }
            
            //StateHasChanged();
        }

        private void LearnCategory(CardCategories category)
        {
            NavigationManager.NavigateTo($"/learn/{category.Name}");
        }

        public MarkupString SplitAt(string str, int interval, int maxInterval)
        {
            if (string.IsNullOrEmpty(str) || interval <= 0 || maxInterval <= 0)
                return new MarkupString(str);

            var result = new List<string>();
            int start = 0;

            while (start < str.Length)
            {
                int end = Math.Min(start + interval, str.Length);
                int nextSpace = str.IndexOf(' ', end);

                if (nextSpace == -1 || nextSpace > start + maxInterval)
                {
                    nextSpace = Math.Min(start + maxInterval, str.Length);
                }

                result.Add(str.Substring(start, nextSpace - start));
                start = nextSpace + 1;
            }

            return new MarkupString(string.Join("<br />", result));
        }

        private async void DeleteCategory()
        {
            try
            {
                await CardCategoriesService.DeleteCategoryAsync(DeletableObject.KategorieId);
                foreach(var card in DeletableCards)
                {
                    await IndexCardService.DeleteIndexCardAsync(card.KategorieId);
                    _cards.Remove(card);
                }
                _categories.Remove(DeletableObject);
                DeleteCategoryBool = false;
                StateHasChanged();
            }
            catch
            {
                Console.WriteLine("Fehler");
            }
        }
        private void ClosePopUp()
        {
            DeleteCategoryBool = false;
            TimeError = false;
        }

    }
}
