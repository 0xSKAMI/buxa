using Discord;
using Discord.WebSocket;
using DotNetEnv;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

public class Program
{
	private static async Task Main(string[] args)
	{
		Env.Load();
		string token = Env.GetString("TOKEN");

		var client = new DiscordSocketClient();
		await client.LoginAsync(TokenType.Bot, token);
		await client.StartAsync();
		await Task.Delay(-1);

		await client.Ready() += 1;
	}

	private static async Task printConnection()
	{
		Console.WriteLine("[CONNECTED]: server is connected");
	}
}
