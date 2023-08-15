using Discord;
using Discord.WebSocket;
using PlusOneData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PlusOne
{
    public class PlusOne
    {
        public DiscordSocketClient Client { get; set; }

        private const string _PlusOneChannelName = "plus-1";

        public async Task Start(string token)
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            Client = new DiscordSocketClient(config);
            Client.Log += Log;
            Client.MessageReceived += OnMessageReceived;

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            await Task.Yield();
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot || message.Channel.Name != _PlusOneChannelName)
                return;

            await Play(message);
        }

        private async Task Play(SocketMessage message)
        {
            using var context = new PlusOneContext();
            var isValid = false;
            var isNumeric = int.TryParse(message.Content, out var numericValue);
            if (isNumeric)
            {
                var lastValidEntry = await context.GetLastValidEntry();
                var lastValidValue = lastValidEntry?.GetValue() ?? 0;
                var lastValidUserId = lastValidEntry?.UserId;

                isValid =
                    numericValue == (lastValidValue + 1) &&
                    message.Author.Id.ToString() != lastValidUserId;
            }
            await context.CreateEntry(message.Content, message.Author.GlobalName, message.Author.Id.ToString(), isValid);
            if (isValid)
            {
                await message.AddReactionAsync(new Emoji("✅"));
                await FunnyNumbers(message);
            }
            else
            {
                await message.AddReactionAsync(new Emoji("❌"));
                var gameOverMessage = await context.GetRandomGameOverMessage();
                await message.Channel.SendMessageAsync(gameOverMessage.GetMessage());
            }
        }

        private async Task FunnyNumbers(SocketMessage message)
        {
            var value = message.Content;

            if (value.Contains("420"))
            {
                await message.AddReactionAsync(new Emoji("🍁"));
                await message.AddReactionAsync(new Emoji("🔥"));
            }
            if (value.Contains("69"))
            {
                await message.AddReactionAsync(new Emoji("🍆"));
                await message.AddReactionAsync(new Emoji("💦"));
                await message.AddReactionAsync(new Emoji("😩"));
                await message.AddReactionAsync(new Emoji("👉"));
                await message.AddReactionAsync(new Emoji("👌"));
                await message.AddReactionAsync(new Emoji("😏"));
            }
            if (value == "80085" || value == "8008135")
            {
                await message.AddReactionAsync(new Emoji("🍒"));
                await message.AddReactionAsync(new Emoji("🍈"));
                await message.AddReactionAsync(new Emoji("🥥"));
                await message.AddReactionAsync(new Emoji("😏"));
            }
        }
    }
}
