using Microsoft.EntityFrameworkCore;
using NewsAggrigation.DAL;
using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Repositories.ExternalApiRepo
{
    public class ExternalApiRepository : IExternalApiRepository
    {
        private readonly NewsAggregatorDbContext _context;

        public ExternalApiRepository(NewsAggregatorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExternalAPIConfig>> GetAllAsync()
        {
            return await _context.ExternalAPIConfigs
                .Where(config => config.IsEnable)
                .ToListAsync();
        }

        public async Task<ExternalAPIConfig?> GetByIdAsync(int id)
        {
            return await _context.ExternalAPIConfigs
                .FirstOrDefaultAsync(config => config.ExternalAPIId == id && config.IsEnable);
        }

        public async Task<ExternalAPIConfig> AddAsync(ExternalAPIConfig config)
        {
            _context.ExternalAPIConfigs.Add(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task<ExternalAPIConfig> UpdateAsync(ExternalAPIConfig config)
        {
            _context.ExternalAPIConfigs.Update(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var config = await _context.ExternalAPIConfigs.FindAsync(id);
            if (config == null) return false;
            config.IsDeleted = true;
            config.IsEnable = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
