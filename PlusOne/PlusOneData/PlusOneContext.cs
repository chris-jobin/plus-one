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
        public PlusOneContext() : base()
        {
            if (Database.EnsureCreated())
            {
                SeedBasicMessages();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("Data Source=plusone.db");

        public DbSet<PlusOneEntry> PlusOneEntries { get; set; }
        public DbSet<GameOverMessage> GameOverMessages { get; set; }

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

        public async Task<GameOverMessage> GetRandomGameOverMessage()
        {
            var random = new Random();
            var messages = await GameOverMessages.ToListAsync();
            var selection = random.Next(messages.Count);
            return messages[selection];
        }

        private void SeedBasicMessages()
        {
            var messages = new List<string>
            {
                "You lose lmao",
                "jail :(",
                ":joy:"
            };
            foreach (var message in messages)
            {
                GameOverMessages.Add(new GameOverMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = message,
                    Created = DateTime.Now
                });
            }
            SaveChangesAsync();
        }
    }
}
