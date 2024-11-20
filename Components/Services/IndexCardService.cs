using Microsoft.EntityFrameworkCore;

namespace IndexCardWebpage
{
    public class IndexCardService
    {
        private readonly CardContext _context;
        public IndexCardService(CardContext context)
        {
            _context = context;
        }
        // Methode zum Hinzufügen einer IndexCard
        public async Task AddIndexCardAsync(IndexCard indexCard)
        {
            try
            {
                if (indexCard == null)
                {
                    return;
                }

                // Setze die nächste freie Id
                if (indexCard.Id == null)
                {
                    indexCard.Id = 0;
                }
                indexCard.Id = await GetNextIdAsync();

                // Füge die IndexCard zur Datenbank hinzu
                _context.IndexCards.Add(indexCard);
                

                // Speichere die Änderungen in der Datenbank
                var changes = await _context.SaveChangesAsync();
                if (changes > 0)
                {
                    
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Methode zum Ermitteln der nächsten freien Id
        public async Task<int> GetNextIdAsync()
        {
            var maxId = await _context.IndexCards
                                       .OrderByDescending(x => x.Id)
                                       .Select(x => x.Id)
                                       .FirstOrDefaultAsync();

            // Rückgabe der nächsten freien Id (maxId + 1 oder 1, wenn keine Karten vorhanden sind)
            return maxId + 1;
        }

        // Methode zum Abrufen einer IndexCard nach ID
        public async Task<IndexCard> GetIndexCardByIdAsync(int id)
        {
            return await _context.IndexCards.FindAsync(id);
        }

        // Methode zum Abrufen aller IndexCards
        public async Task<List<IndexCard>> GetAllIndexCardsAsync()
        {
            return await _context.IndexCards.ToListAsync();
        }

        // Methode zum Aktualisieren einer IndexCard
        public async Task UpdateIndexCardAsync(IndexCard indexCard)
        {
            _context.IndexCards.Update(indexCard);
            await _context.SaveChangesAsync();
        }

        public int GetIdByIndexCard(IndexCard indexCard)
        {
            var indexCardEntity = _context.IndexCards
                                           .FirstOrDefault(a => a.Description == indexCard.Description && a.KategorieId == indexCard.KategorieId);
            if (indexCardEntity != null)
            {
                return indexCardEntity.Id; 
            }
            else
            {
                throw new Exception("IndexCard not found.");
            }
        }

        public async Task<List<IndexCard>> GetIndexCardsByCategoryID(int categoryID)
        {
            return await _context.IndexCards
                                 .Where(c => c.KategorieId == categoryID)
                                 .ToListAsync();
        }

        // Methode zum Löschen einer IndexCard
        public async Task DeleteIndexCardAsync(int id)
        {
            var indexCard = await _context.IndexCards.FindAsync(id);
            if (indexCard != null)
            {
                _context.IndexCards.Remove(indexCard);
                await _context.SaveChangesAsync();
            }
        }
    }
}
