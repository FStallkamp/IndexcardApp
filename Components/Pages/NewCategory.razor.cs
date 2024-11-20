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
            var dialog =  await DialogService.ShowAsync<NewCardDialog>("Neue Kartei-Karte", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {

                var newCard = (IndexCard)result.Data;
                if (newCard != null)
                {
                    Cards.Add(newCard); // Zum Beispiel: Jede Karte zu einer bestehenden Liste hinzufügen
                }

            }
        }

        public async Task AddCategoryAsync(CardCategories newCategory)
        {
            await CardCategoriesService.AddCategoryAsync(newCategory);

            newCategory = new CardCategories();
        }

        public async Task AddIndexCardAsync(IndexCard indexCard)
        {
            await IndexCardService.AddIndexCardAsync(indexCard);

            indexCard = new IndexCard();
        }

        private async Task Save()
        {
            if (Cards != null)
            {
                foreach (var card in Cards)
                {
                    AddIndexCardAsync (card);
                }
            }
            if (Title == null)
            {
                TitleMissed = true;
            }
            else if (Description == null)
            {
                DescriptionMissed = true;
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
                AddCategoryAsync(category);
                
            }
        }

        // Edit-Funktion vorerst ausgebaut
        /*  
        private async void Edit(IndexCard card)
        {
            int cardId = IndexCardService.GetIdByIndexCard(card);
            var parameters = new DialogParameters
            {
                { "Id", card.Id },
                { "KatID", KategorieID }, 
                { "Title", card.Name },
                { "Description", card.Description }
            };
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = DialogService.Show<NewCardDialog>("Neue Kartei-Karte", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var oldCard = await IndexCardService.GetIndexCardByIdAsync(cardId);
                var newCard = (IndexCard)result.Data;
                
                if (newCard != null)
                {
                    Cards.Add(newCard); // Zum Beispiel: Jede Karte zu einer bestehenden Liste hinzufügen
                }
                Cards.Remove(oldCard);

            }
        }
        */
    }
}
