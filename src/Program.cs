using Discord; // Discord API for bot interaction
using Discord.WebSocket; // Provides the WebSocket-based client
using DotNetEnv; // To load environment variables from a `.env` file using System; // Basic system functionalities like Console output
using System.Net.Http;
using SteamN;


public class Program
{   
	// Initialize the Discord socket client with the configuration
	public static DiscordSocketClient _client = new DiscordSocketClient
	(
		new DiscordSocketConfig 
		{ 		 
			GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildPresences | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
		}
	);
	public static Steam something = new Steam();

	// Main entry point of the application
	private static async Task Main(string[] args)
	{	
		Env.Load();
		something.Connect();

		// Retrieve the bot token from the environment variables
		string token = Env.GetString("TOKEN");

		// Subscribe to events that the bot should listen to
		_client.Log += LogMessage; // Event triggered for logging messages from the bot
		_client.Ready += Create_command;
		_client.SlashCommandExecuted += SlashCommandHandler;

		// Log in the bot with the token a:wnd start the connection
		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();

		// Keep the bot running indefinitely
		await Task.Delay(-1);
	}

	// This method is called whenever a new log message is generated (for debugging or logging purposes)
	private static async Task LogMessage(LogMessage message)
	{
		// Print the log message to the console
		Console.WriteLine(message);

		await Task.CompletedTask;
	}
	
	//method to create commands 
	private static async Task Create_command() 
	{
		//create command builder and give it description and name
		var commandBuilder = new SlashCommandBuilder();
		
		commandBuilder.WithName("connect");
		commandBuilder.WithDescription("connects steam account to discord account");
		commandBuilder.AddOption("steam_id", ApplicationCommandOptionType.String, "steam id", isRequired: true);
		
		//try to build command and if something goes wrong write it in console
		try
		{
			await _client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
		}
		catch 
		{
			Console.WriteLine("Creating commands went wrong");	
		}
		await Task.CompletedTask;
	}

	private static async Task SlashCommandHandler(SocketSlashCommand command)
	{
		switch(command.Data.Name)
		{
			case "connect":
				await command.RespondAsync($"{command.User}");
				break;
		}
		// We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
    var guildUser = command.Data.Options.First().Value;

    // Now, Let's respond with the embed.
    await command.RespondAsync(Convert.ToString(guildUser));
	}
}
