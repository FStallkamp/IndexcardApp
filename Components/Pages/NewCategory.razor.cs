using Microsoft.AspNetCore.Components;
using MudBlazor;
using IndexCardWebpage.Components.Dialogs;
using System.Diagnostics.Metrics;
using static IndexCardWebpage.Components.Pages.Home;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IndexCardWebpage.Components.Pages
{
    public partial class NewCategory
    {
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] CardCategoriesService CardCategoriesService { get; set; }
        [Inject] IndexCardService IndexCardService { get; set; }

        List<IndexCard> Cards = new List<IndexCard>();

        string Title = "";
        string Description = "";
        private int KategorieID;
        private string Ersteller = "";
        private bool TitleMissed = false;
        private bool DescriptionMissed = false;
        private bool TitleExists = false;

        protected override async Task OnInitializedAsync()
        {
            KategorieID = await CardCategoriesService.GetNextCategoryIdAsync();
        }

        private async Task OpenDialog()
        {
            var parameters = new DialogParameters
            {
                { "KatID", KategorieID }  // Übergibt die KategorieId an den Dialog
            };
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = await DialogService.ShowAsync<NewCardDialog>("Neue Kartei-Karte", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var newCard = (IndexCard)result.Data;
                if (newCard != null && !string.IsNullOrWhiteSpace(newCard.Name) && !string.IsNullOrWhiteSpace(newCard.Description))
                {
                    Cards.Add(newCard); // Zum Beispiel: Jede Karte zu einer bestehenden Liste hinzufügen
                }
            }
        }

        public async Task AddCategoryAsync(CardCategories newCategory)
        {
            if (!string.IsNullOrWhiteSpace(newCategory.Name))
            {
                await CardCategoriesService.AddCategoryAsync(newCategory);
                newCategory = new CardCategories();
            }
        }

        public async Task AddIndexCardAsync(IndexCard indexCard)
        {
            if (!string.IsNullOrWhiteSpace(indexCard.Name) && !string.IsNullOrWhiteSpace(indexCard.Description))
            {
                await IndexCardService.AddIndexCardAsync(indexCard);
                indexCard = new IndexCard();
            }
        }

        private async Task Save()
        {
            if (Cards != null)
            {
                foreach (var card in Cards)
                {
                    await AddIndexCardAsync(card);
                }
            }
            if (string.IsNullOrWhiteSpace(Title))
            {
                TitleMissed = true;
            }
            else if (TitleExists)
            {
                // Fehler anzeigen, dass der Titel bereits existiert
                return;
            }
            else
            {
                var category = new CardCategories
                {
                    Name = Title,
                    Beschreibung = Description,
                    KategorieId = KategorieID,
                    Ersteller = Ersteller
                };
                await AddCategoryAsync(category);
            }
        }

        private async Task CheckTitle()
        {
            var categories = await CardCategoriesService.GetAllCategoriesAsync();
            TitleExists = categories.Any(c => c.Name.Equals(Title, StringComparison.OrdinalIgnoreCase));
        }
    }
}
