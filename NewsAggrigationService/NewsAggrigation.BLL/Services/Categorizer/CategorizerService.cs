using Microsoft.EntityFrameworkCore;
using NewsAggrigation.DAL;

namespace NewsAggrigation.BLL.Services.Categorizer
{
    public class CategorizerService : ICategorizerService
    {
        private readonly NewsAggregatorDbContext _context;

        public CategorizerService(NewsAggregatorDbContext context)
        {
            _context = context;
        }

        public async Task<int?> DetectCategoryAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var categoryKeywords = await _context.CategoryKeywords
                .AsNoTracking()
                .Where(k => !k.IsDeleted)
                .ToListAsync();

            if (!categoryKeywords.Any())
                return null;

            var categoryScores = new Dictionary<int, int>();

            foreach (var keyword in categoryKeywords)
            {
                if (string.IsNullOrWhiteSpace(keyword.KeywordName))
                    continue;

                if (content.Contains(keyword.KeywordName, StringComparison.OrdinalIgnoreCase))
                {
                    if (categoryScores.ContainsKey(keyword.CategoryId))
                        categoryScores[keyword.CategoryId]++;
                    else
                        categoryScores[keyword.CategoryId] = 1;
                }
            }

            // If nothing matched
            if (!categoryScores.Any())
                return null;

            return categoryScores
                .OrderByDescending(kvp => kvp.Value)
                .First()
                .Key;
        }
    }
}
