using IndexCardWebpage.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using static MudBlazor.CategoryTypes;
namespace IndexCardWebpage.Components.Pages
{
    public partial class EditCategory
    {
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] CardCategoriesService CardCategoriesService { get; set; }
        [Inject] IndexCardService IndexCardService { get; set; }

        [Parameter]
        public string CategoryName { get; set; }
        private bool CardsError = false;
        private bool TimeError = false;
        private bool DeleteCardPopUp = false;
        private int DeleteTimer = 10;
        private DateTime _lastDeleteAttempt = DateTime.MinValue;
        private List<IndexCard> DeletableCards = new List<IndexCard>();
        private List<IndexCard> indexCards;
        private CardCategories Category;
        private List<IndexCard> localCards = new List<IndexCard>();

        private bool CategoryNameExists = false;

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
            NavigationManager.NavigateTo("/?refresh=true");
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
                NavigationManager.NavigateTo("/?refresh=true");
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
            // 

            StateHasChanged();
            NavigationManager.NavigateTo("/?refresh=true");
        }

        private async Task OpenDialog()
        {
            var parameters = new DialogParameters
            {
                { "KatID", Category.KategorieId }  // Übergibt die KategorieId an den Dialog
            };
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = await DialogService.ShowAsync<NewCardDialog>("Neue Kartei-Karte", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var newCard = (IndexCard)result.Data;
                if (newCard != null && !string.IsNullOrWhiteSpace(newCard.Name) && !string.IsNullOrWhiteSpace(newCard.Description))
                {
                    indexCards.Add(newCard); // Zum Beispiel: Jede Karte zu einer bestehenden Liste hinzufügen
                }
            }
        }

        private async Task DeleteCard(IndexCard card)
        {
            {
                try
                {

                    DeletableCards.Add(card);

                    DeleteCardPopUp = true;
                }
                catch
                {
                    Console.WriteLine("Kategorie nicht gefunden!");
                }

                //StateHasChanged();
            }
        }

        private async void CompleteDelete()
        {

            if ((DateTime.Now - _lastDeleteAttempt).TotalSeconds < DeleteTimer)
            {
                TimeError = true;
                StateHasChanged(); // UI aktualisieren
                return;
            }

            _lastDeleteAttempt = DateTime.Now;

            try
            {
                foreach (var card in DeletableCards)
                {
                    await IndexCardService.DeleteIndexCardAsync(card.KategorieId);
                    indexCards.Remove(card);
                }
                DeleteCardPopUp = false;
                StateHasChanged();
            }
            catch
            {
                Console.WriteLine("Fehler");
            }

        }

        private async Task CheckCategoryName()
        {
            var categories = await CardCategoriesService.GetAllCategoriesAsync();
            CategoryNameExists = categories.Any(c => c.Name.Equals(Category.Name, StringComparison.OrdinalIgnoreCase) && c.KategorieId != Category.KategorieId);
        }
    }
}
