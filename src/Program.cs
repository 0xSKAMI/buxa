using Discord; // Discord API for bot interaction
using Discord.WebSocket; // Provides the WebSocket-based client
using DotNetEnv; // To load environment variables from a `.env` file using System; // Basic system functionalities like Console output
using System.Net.Http;
using SteamN;
using System.Diagnostics; //This namespace is used to open links in browser
using DB;
using Handler;

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
	public static Steam stm = new Steam();
	public static Player plr = new Player();

	// Main entry point of the application
	private static async Task Main(string[] args)
	{	
		Env.Load();
		stm.Connect();

		// Retrieve the bot token from the environment variables
		string token = Env.GetString("TOKEN");

		// Subscribe to events that the bot should listen to
		_client.Log += LogMessage; // Event triggered for logging messages from the bot
		_client.Ready += Create_command;
		_client.SlashCommandExecuted += SlashCommandHandler;

		// Log in the bot with the token a:wnd start the connection
		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();
		
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

	//handler for slash commands
	private static async Task SlashCommandHandler(SocketSlashCommand command)
	{
		switch(command.Data.Name)
		{
			case "connect":
				//declare steamId to save steam ID there
				string steamId = "";
				//reply to user
				await command.RespondAsync("Sign in with your steam account");
				//open link in broser
				Process.Start(new ProcessStartInfo
				{
						FileName = "https://steamcommunity.com/openid/login?openid.ns=http://specs.openid.net/auth/2.0&openid.mode=checkid_setup&openid.return_to=http://127.0.0.1:3000/return/&openid.realm=http://127.0.0.1:3000/&openid.identity=http://specs.openid.net/auth/2.0/identifier_select&openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select",
						UseShellExecute = true
				});
				//start listening to port and when user signs up update database
				_= Task.Run(async () => {
					string steamId = await Steam.ListenToPort();
					await plr.ConnectPlayer(command.User.AvatarId, long.Parse(steamId));
				});
				break;
		}
	}
}
