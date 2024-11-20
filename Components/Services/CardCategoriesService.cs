using IndexCardWebpage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CardCategoriesService
{
    private readonly CardContext _context;
    private readonly ILogger<CardCategoriesService> _logger;

    public CardCategoriesService(CardContext context, ILogger<CardCategoriesService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Methode zum Abrufen der nächsten freien KategorieId
    public async Task<int> GetNextCategoryIdAsync()
    {
        var maxCategoryId = await _context.CardCategories
                                          .OrderByDescending(c => c.KategorieId)
                                          .Select(c => c.KategorieId)
                                          .FirstOrDefaultAsync();

        // Rückgabe der nächsten freien KategorieId (maxCategoryId + 1 oder 1, wenn keine Kategorien vorhanden sind)
        return maxCategoryId + 1;
    }

    // Methode zum Abrufen aller Kategorien
    public async Task<List<CardCategories>> GetAllCategoriesAsync()
    {
        try
        {
            return await _context.CardCategories.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der Kategorien.");
            throw;
        }
    }

    // Methode zum Abrufen einer Kategorie nach ID
    public async Task<CardCategories> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _context.CardCategories
                                 .FirstOrDefaultAsync(c => c.KategorieId == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der Kategorie.");
            throw;
        }
    }

    // Methode zum Hinzufügen einer neuen Kategorie
    public async Task AddCategoryAsync(CardCategories category)
    {
        try
        {
            // Setze die nächste freie KategorieId
            category.KategorieId = await GetNextCategoryIdAsync();

            // Füge die neue Kategorie in die Datenbank ein
            _context.CardCategories.Add(category);

            // Speichere die Änderungen in der Datenbank
            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                _logger.LogInformation("Kategorie erfolgreich hinzugefügt: {CategoryName}", category.Name);
            }
            else
            {
                _logger.LogWarning("Keine Änderungen wurden in der Datenbank gespeichert.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Hinzufügen der Kategorie.");
            throw;
        }
    }

    public async Task<int> GetCategoryIDbyCategoryName(string categoryName)
    {
        var category = await _context.CardCategories
                                      .FirstOrDefaultAsync(c => c.Name == categoryName);
        if (category != null)
        {
            return category.KategorieId;  // Oder wie auch immer die ID der Kategorie gespeichert wird
        }
        else
        {
            throw new Exception("Kategorie nicht gefunden.");
        }
    }



    // Methode zum Aktualisieren einer Kategorie
    public async Task UpdateCategoryAsync(CardCategories category)
    {
        try
        {
            _context.CardCategories.Update(category);
            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                _logger.LogInformation("Kategorie erfolgreich aktualisiert: {CategoryName}", category.Name);
            }
            else
            {
                _logger.LogWarning("Keine Änderungen wurden in der Datenbank gespeichert.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Aktualisieren der Kategorie.");
            throw;
        }
    }

    // Methode zum Löschen einer Kategorie
    public async Task DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _context.CardCategories
                                         .FirstOrDefaultAsync(c => c.KategorieId == id);

            if (category != null)
            {
                _context.CardCategories.Remove(category);
                var changes = await _context.SaveChangesAsync();
                if (changes > 0)
                {
                    _logger.LogInformation("Kategorie erfolgreich gelöscht: {CategoryName}", category.Name);
                }
                else
                {
                    _logger.LogWarning("Keine Änderungen wurden in der Datenbank gespeichert.");
                }
            }
            else
            {
                _logger.LogWarning("Kategorie mit ID {CategoryId} wurde nicht gefunden.", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Löschen der Kategorie.");
            throw;
        }
    }

    public async Task<CardCategories> GetCategoryByNameAsync(string categoryName)
    {
        return await _context.CardCategories.FirstOrDefaultAsync(c => c.Name == categoryName);
    }

}
