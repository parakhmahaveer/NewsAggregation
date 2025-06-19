using Microsoft.EntityFrameworkCore;
using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL
{
    public class NewsAggregatorDbContext : DbContext
    {
        public NewsAggregatorDbContext(DbContextOptions<NewsAggregatorDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<SavedArticle> SavedArticles { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Keyword> Keywords { get; set; }

        public DbSet<NotificationSetting> NotificationSettings { get; set; }

        public DbSet<ExternalAPIConfig> ExternalAPIConfigs { get; set; }

        public DbSet<CategoryKeyword> CategoryKeywords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply soft delete global filters
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Article>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Keyword>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SavedArticle>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<NotificationSetting>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ExternalAPIConfig>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CategoryKeyword>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }
    }
}
