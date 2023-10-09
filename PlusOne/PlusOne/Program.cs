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
            var path = $"{Directory.GetCurrentDirectory()}/plusone.json";
            var node = JsonNode.Parse(File.ReadAllText(path));
            var token = node["token"];
            return token.ToString();
        }
    }
}