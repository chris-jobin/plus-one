using Microsoft.EntityFrameworkCore;
using PlusOneData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusOneData
{
    public class PlusOneContext : DbContext
    {
        private static bool Created { get; set; }

        public PlusOneContext() : base()
        {
            if (!Created)
            {
                Database.EnsureCreated();
                Created = true;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("Data Source=plusone.db");

        public DbSet<PlusOneEntry> PlusOneEntries { get; set; }

        public async Task<PlusOneEntry> GetLastValidEntry()
        {
            var result = await PlusOneEntries.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            return (result?.IsValid ?? false) ? result : null;
        }

        public async Task CreateEntry(string value, string username, string userId, bool isValid)
        {
            await PlusOneEntries.AddAsync(new PlusOneEntry
            {
                Id = Guid.NewGuid().ToString(),
                Value = value,
                UserName = username,
                UserId = userId,
                IsValid = isValid,
                Created = DateTime.Now
            });
            await SaveChangesAsync();
        }
    }
}
