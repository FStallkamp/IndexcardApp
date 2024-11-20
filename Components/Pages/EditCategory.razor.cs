using Microsoft.AspNetCore.Components;
using static MudBlazor.CategoryTypes;
namespace IndexCardWebpage.Components.Pages
{
    public partial class EditCategory
    {
        [Inject] CardCategoriesService CardCategoriesService { get; set; }
        [Inject] IndexCardService IndexCardService { get; set; }

        [Parameter]
        public string CategoryName { get; set; }
        private bool CardsError = false;
        private List<IndexCard> indexCards;
        private CardCategories Category;
        private List<IndexCard> localCards = new List<IndexCard>();

        protected override async Task OnInitializedAsync()
        {
            
            if (!string.IsNullOrEmpty(CategoryName))
            {
                // Lade die Kategorie anhand des Namens oder anderer eindeutiger Eigenschaften
                Category = await CardCategoriesService.GetCategoryByNameAsync(CategoryName);
            }
            try
            {
                indexCards = await IndexCardService.GetAllIndexCardsAsync();
            }
            catch
            {

            }
            var cards = await IndexCardService.GetAllIndexCardsAsync();
           

            // localCards ebenfalls als Kopie initialisieren
            localCards = indexCards.Select(card => new IndexCard
            {
                Id = card.Id,
                Name = card.Name,
                Description = card.Description,
                KategorieId = card.KategorieId
            }).ToList();
        }

        private async Task SaveCategory()
        {
            if (Category != null)
            {
                await CardCategoriesService.UpdateCategoryAsync(Category);
            }

            foreach (var card in indexCards)
            {
                if (card != null)
                {
                    await IndexCardService.UpdateIndexCardAsync(card);
                }
            }

            // Nach Speichern zum Hauptbildschirm
            NavigationManager.NavigateTo("/");
        }

        private async Task Cancel()
        {
            // Prüfen, ob es Änderungen gibt
            bool hasChanges = localCards.Any(c =>
                c.Name != indexCards.FirstOrDefault(ic => ic.Id == c.Id)?.Name ||
                c.Description != indexCards.FirstOrDefault(ic => ic.Id == c.Id)?.Description);

            if (hasChanges)
            {
                // Popup anzeigen
                CardsError = true;
            }
            else
            {
                // Keine Änderungen -> Navigieren
                NavigationManager.NavigateTo("/");
            }
        }

        private async Task ConfirmSave()
        {
            // Änderungen speichern und Popup schließen
            await SaveCategory();
            CardsError = false;
        }

        private void DiscardChanges()
        {
            // indexCards vollständig durch eine Kopie von localCards ersetzen
            indexCards = localCards.Select(card => new IndexCard
            {
                Id = card.Id,
                Name = card.Name,
                Description = card.Description,
                KategorieId = card.KategorieId
            }).ToList();

            // Popup schließen (falls verwendet)
            CardsError = false;

            // UI aktualisieren
            StateHasChanged();
        }


    }
}
