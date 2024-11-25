using Microsoft.AspNetCore.Components;
using System;

namespace IndexCardWebpage.Components.Pages
{
    public partial class Learn
    {
        [Inject]
        CardCategoriesService CardCategoriesService { get; set; }
        [Inject]
        IndexCardService IndexCardService { get; set; }
        [Parameter]
        public string CategoryName { get; set; }
        private int KategorieID;
        private List<IndexCard> indexCards;
        private List<IndexCard> randomOrder;
        private string inputUser;
        private bool StepTwo = false;
        private int cardCounter = 0;
        private IndexCard currentCard = new IndexCard();
        Random rnd = new Random();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Hole die Kategorie-ID anhand des CategoryNames
                KategorieID = await CardCategoriesService.GetCategoryIDbyCategoryName(CategoryName);

                // Hole die Indexkarten für die Kategorie, basierend auf der KategorieID
                indexCards = await IndexCardService.GetIndexCardsByCategoryID(KategorieID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Daten: {ex.Message}");
            }
            GenerateRandomList(indexCards);
        }

        private void GenerateRandomList(List<IndexCard> cards)
        {
            randomOrder = cards  // Benutze die schon existierende randomOrder-Liste
                .OrderBy(x => rnd.Next())  // Zufällig ordnen
                .ToList();
            cardCounter = randomOrder.Count();
            if(currentCard.Name == null)
            {
                currentCard = randomOrder.FirstOrDefault();  // Erste Karte auswählen
            }
            
        }


        private async Task StartStepTwo(List<IndexCard> randomOrder)
        {
            StepTwo = true;
        }

        private async Task WrongAnswer()
        {
            randomOrder = randomOrder  // Benutze die schon existierende randomOrder-Liste
                .OrderBy(x => rnd.Next())  // Zufällig ordnen
                .ToList();
            currentCard = randomOrder.FirstOrDefault();
            inputUser = "";
            StepTwo = false;
        }

        private async Task CorrectAnswer(IndexCard oldCard)
        {
            cardCounter--;
            randomOrder.Remove(oldCard);
            currentCard = randomOrder.FirstOrDefault();
            inputUser = "";
            StepTwo = false;
        }

    }
}
