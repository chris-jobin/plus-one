using Discord;
using Discord.WebSocket;
using PlusOne.Data;
using System.Text.Json.Nodes;

namespace PlusOne
{
    public static class PlusOne
    {
        private const string _plusoneChannelName = "plus-1";
        private static DiscordSocketClient _client;

        public static async Task Start()
        {
            var token = GetToken();

            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                MessageCacheSize = 3
            };

            _client = new DiscordSocketClient(config);
            _client.Ready += Ready;
            _client.Log += Log;

            _client.MessageReceived += OnMessageReceived;
            _client.MessageDeleted += OnMessageDeleted;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static string GetToken()
        {
            var path = Directory.GetCurrentDirectory() + "/plusone.json";
            var json = File.ReadAllText(path);
            var node = JsonNode.Parse(json);
            var token = node["token"];

            return token.ToString();
        }

        private static async Task Ready()
        {
            await Task.Yield();
            Console.WriteLine("+1 is logged in!");
        }

        private static async Task Log(LogMessage message)
        {
            await Task.Yield();
            Console.WriteLine(message.ToString());
        }

        private static async Task OnMessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot || !message.Channel.Name.Contains(_plusoneChannelName))
            {
                return;
            }

            await Play(message);
        }

        private static async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            if (!channel.Value.Name.Contains(_plusoneChannelName))
            {
                return;
            }

            var socketMessage = (message.Value as SocketMessage);

            if (socketMessage is null)
            {
                return;
            }

            if (socketMessage.Author.Username == _client.CurrentUser.Username && socketMessage.Content.StartsWith("Previous message was deleted!"))
            {
                await socketMessage.Channel.SendMessageAsync(socketMessage.Content);
            }

            await MessageDeleted(socketMessage);
        }

        private static async Task Play(SocketMessage message)
        {
            using var context = new PlusOneContext();

            var author = message.Author;
            var channel = message.Channel;
            var content = message.Content;
            var isValid = false;
            var isNumeric = int.TryParse(content, out var numericValue);

            if (isNumeric)
            {
                var lastValidEntry = await context.GetLastChannelEntry(channel.Id.ToString());
                var lastValidValue = lastValidEntry?.GetValue() ?? 0;
                var lastValidUserId = lastValidEntry?.UserId;

                isValid =
                    numericValue == (lastValidValue + 1) &&
                    author.Id.ToString() != lastValidUserId;
            }

            await context.CreateEntry(
                channelId: channel.Id.ToString(),
                messageId: message.Id.ToString(),
                userId: author.Id.ToString(),
                username: author.GlobalName,
                value: content,
                isValid: isValid);

            if (isValid)
            {
                await message.AddReactionAsync(new Emoji("✅"));
                await FunnyNumbers(message);
            }
            else
            {
                await message.AddReactionAsync(new Emoji("❌"));
                var gameOverMessage = await context.GetRandomGameOverMessage(channel.Id.ToString());
                var messageReference = new MessageReference(message.Id);
                await channel.SendMessageAsync(gameOverMessage.Message, messageReference: messageReference);
            }
        }

        private static async Task FunnyNumbers(SocketMessage message)
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

            if (value == "8008" || value == "80085" || value == "8008135")
            {
                await message.AddReactionAsync(new Emoji("🍒"));
                await message.AddReactionAsync(new Emoji("🍈"));
                await message.AddReactionAsync(new Emoji("🥥"));
                await message.AddReactionAsync(new Emoji("😏"));
            }
        }

        private static async Task MessageDeleted(SocketMessage message)
        {
            using var context = new PlusOneContext();

            var messageId = message.Id;
            var channel = message.Channel;
            var lastValidEntry = await context.GetLastChannelEntry(channel.Id.ToString());

            if (lastValidEntry is null || lastValidEntry.MessageId != messageId.ToString())
            {
                return;
            }

            var author = message.Author;
            var value = message.Content;
            var notification = $"Previous message was deleted! The last valid entry was **{value}**, sent by **{author.GlobalName}**.";

            await channel.SendMessageAsync(notification);
        }
    }
}
