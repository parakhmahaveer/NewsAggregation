using Microsoft.EntityFrameworkCore;
using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Repositories.NotificationRepo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NewsAggregatorDbContext _context;

        public NotificationRepository(NewsAggregatorDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.Article)
                .Include(n => n.User)
                .Where(n => n.User.UserId == userId && !n.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }

        public async Task<CategoryNotificationSetting?> GetCategorySettingAsync(int userId, int categoryId)
        {
            return await _context.CategoryNotificationSettings
                .FirstOrDefaultAsync(s => s.UserId == userId && s.CategoryId == categoryId);
        }

        public async Task AddCategorySettingAsync(CategoryNotificationSetting setting)
        {
            _context.CategoryNotificationSettings.Add(setting);
        }

        public async Task<List<Keyword>> GetUserKeywordsAsync(int userId)
        {
            return await _context.Keywords.Where(k => k.UserId == userId).ToListAsync();
        }

        public async Task AddKeywordsAsync(IEnumerable<Keyword> keywords)
        {
            await _context.Keywords.AddRangeAsync(keywords);
        }

        public async Task RemoveKeywordsAsync(IEnumerable<Keyword> keywords)
        {
            _context.Keywords.RemoveRange(keywords);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserCategoryNotificationPreferencesAsync(int userId)
        {
            return await _context.CategoryNotificationSettings
                .Where(c => c.UserId == userId && c.IsEnabled && !c.IsDeleted)
                .Select(c => c.Category.CategoryName)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserKeywordNotificationPreferencesAsync(int userId)
        {
            return await _context.Keywords
                .Where(k => k.UserId == userId && !k.IsDeleted)
                .Select(k => k.Word)
                .ToListAsync();
        }

        public async Task<bool> HideNotificationAsync(List<Notification> notifications)
        {
            var toHide = notifications.Take(5).ToList();
            foreach (var notification in toHide)
            {
                notification.IsDeleted = true;
                _context.Notifications.Update(notification);
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
