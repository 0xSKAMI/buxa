using Discord; // Discord API for bot interaction
using Discord.WebSocket; // Provides the WebSocket-based client
using DotNetEnv; // To load environment variables from a `.env` file
using System; // Basic system functionalities like Console output
using System.Threading.Tasks; // For asynchronous programming
using Discord.Commands;
using Microsoft.Win32.SafeHandles; // For command handling (though not used here)

public class Program
{	
	public static List<User> users = new List<User>();

	// Main entry point of the application
	private static async Task Main(string[] args)
	{
		// Load environment variables from .env file (such as the bot token)
		Env.Load();

		// Configure the bot client with necessary intents to track guilds, members, and presence updates
		var socketConfig = new DiscordSocketConfig
		{ 		 
			GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildPresences | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
		};

		// Initialize the Discord socket client with the configuration
		var _client = new DiscordSocketClient(socketConfig);

		// Retrieve the bot token from the environment variables
		string token = Env.GetString("TOKEN");

		// Subscribe to events that the bot should listen to
		_client.PresenceUpdated += PrintActivity; // Event triggered when a user's presence changes (activity, status, etc.)
		_client.Log += LogMessage; // Event triggered for logging messages from the bot

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

	// This method is called when a user's activity or status changes (presence update)
 private static async Task PrintActivity(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
	{
		Console.WriteLine("saba");

		if(oldPresence.Activities.Any() && !newPresence.Activities.Any())
		{
			foreach(var activity in oldPresence.Activities)
			{
				// Print a message indicating that a user has started a new activity
				if(Convert.ToString(activity.Type) == "Playing")
				{
					User localUser = new User(Convert.ToString(user), Convert.ToString(activity));
					if (users.Any(e => e.userName ==  localUser.userName && e.game == localUser.game))
					{
						int index = users.FindIndex(u => u.userName == localUser.userName && u.game == localUser.game);
						Console.WriteLine(DateTime.Now.Subtract(users[index].date));
						users.RemoveAt(index);
					}
				};
			}
		}

		if(user.Activities.Any())
		{
			foreach(var activity in user.Activities)
			{
				// Print a message indicating that a user has started a new activity
				if(Convert.ToString(activity.Type) == "Playing")
				{
					User localUser = new User(Convert.ToString(user), Convert.ToString(activity));
					users.Add(localUser);
				}
			}
		}
	}
}


public class User
{
	public string userName {get; }
	public DateTime date {get; }
	public string game {get; }

	public User(string UserName, string Game)
	{
		userName = UserName;
		date = DateTime.Now;
		Game = game;
	}
}