using System.Text.Json.Nodes;

namespace PlusOne
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new PlusOne();
            var token = GetToken();
            await bot.Start(token);
        }

        static string GetToken()
        {
            var node = JsonNode.Parse(File.ReadAllText(@"./plusone.json"));
            var token = node["token"];
            return token.ToString();
        }
    }
}