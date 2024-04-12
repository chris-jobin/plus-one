using Microsoft.EntityFrameworkCore;
using PlusOneData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusOne.Data
{
    public class PlusOneContext : DbContext
    {
        private const string _databaseName = "plusone.db";

        public PlusOneContext() : base()
        {
            _ = Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + _databaseName);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameOverMessage>().HasData(
                    new GameOverMessage { Id = Guid.NewGuid().ToString(), Created = DateTime.Now, Message = "You lose lmao" },
                    new GameOverMessage { Id = Guid.NewGuid().ToString(), Created = DateTime.Now, Message = "jail :(" },
                    new GameOverMessage { Id = Guid.NewGuid().ToString(), Created = DateTime.Now, Message = ":joy:" });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PlusOneEntry> PlusOneEntries { get; set; }
        public DbSet<GameOverMessage> GameOverMessages { get; set; }

        public async Task<PlusOneEntry> GetLastChannelEntry(string channelId)
        {
            var lastValidEntry = await PlusOneEntries
                .Where(x => x.ChannelId == channelId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            var result = (lastValidEntry?.IsValid ?? false) ? lastValidEntry : null;

            return result;
        }

        public async Task CreateEntry(string channelId, string userId, string username, string value, bool isValid)
        {
            await PlusOneEntries.AddAsync(new PlusOneEntry
            {
                Id = Guid.NewGuid().ToString(),
                ChannelId = channelId,
                UserId = userId,
                UserName = username,
                Value = value,
                IsValid = isValid,
                Created = DateTime.Now
            });

            await SaveChangesAsync();
        }

        public async Task<GameOverMessage> GetRandomGameOverMessage(string channelId)
        {
            var messages = await GameOverMessages
                .Where(x => x.ChannelId == null || x.ChannelId == channelId)
                .ToListAsync();

            if (messages.Count == 0)
            {
                return new GameOverMessage
                {
                    Message = "You lose",
                    Created = DateTime.Today
                };
            }

            var random = new Random();
            var selection = random.Next(messages.Count);

            return messages[selection];
        }
    }
}
