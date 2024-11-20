using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using IndexCardWebpage.Components.Pages;
using System.Text;

namespace IndexCardWebpage.Components.Dialogs
{
    public partial class NewCardDialog
    {
        private readonly CardContext _context;
        private bool TitleMissed = false;
        private bool DescriptionMissed = false;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        
        [Inject] IndexCardService IndexCardService { get; set; }

        [Parameter] public int KatID { get; set; }

        private string Title = "";
        private string Description = "";
         
        private void Submit()
        {
                if(Title == null)
                {
                    TitleMissed = true;
                }
                else if (Description == null)
                {
                    DescriptionMissed = true;
                }
                else
                {
                    var indexCard = new IndexCard
                    {
                        Name = Title,
                        Description = Description,
                        KategorieId = KatID
                    };
                    MudDialog.Close(DialogResult.Ok(indexCard));
                   // AddIndexCardAsync(indexCard);
                }
        }

        private void EditCard()
        {
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
                var indexCard = new IndexCard
                {
                    Name = Title,
                    Description = Description,
                    KategorieId = KatID
                };
                MudDialog.Close(DialogResult.Ok(indexCard));
                // AddIndexCardAsync(indexCard);
            }
        }

        private void Cancel() => MudDialog.Close(DialogResult.Cancel());

       
        
    }
}

