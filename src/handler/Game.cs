using DB;
using System.Data.Common;
using SteamN;
using System.Text.Json;


namespace Handler
{
	//class where most of game related methods will be
	public class Game
	{
		//initilize steam object
		public static Steam stm = new Steam();
	
		//this method adds game to database or if it already exists just updates time
		public async Task AddGames(string discordId, long steamId)
		{
			//creating database instance
			Database db = Database.Instance;
	
			//get user's games in steam library
			var root = await stm.GetGames(Convert.ToString(steamId));
			var games = root.EnumerateArray();
			
			var tasks = new List<Task>();

			//start a loop
			while (games.MoveNext())
			{
				var game = games.Current;

				if (game.GetProperty("playtime_forever").ToString() != "0")
				{
					//make a tasks so it will run in background
					tasks.Add(Task.Run(async () =>
					{
						int appId = game.GetProperty("appid").GetInt32();
						int playtime = game.GetProperty("playtime_forever").GetInt32();
						int playtime_windows = game.GetProperty("playtime_windows_forever").GetInt32();
						int playtime_mac = game.GetProperty("playtime_mac_forever").GetInt32();
						int playtime_linux = game.GetProperty("playtime_linux_forever").GetInt32();
						int playtime_deck = game.GetProperty("playtime_deck_forever").GetInt32();
						string name = game.GetProperty("name").GetString();

						if (await db.GetGame(appId) == "1")
						{
							await db.UpdateGameTimeAdd(appId, playtime);
						}
						else
						{
							await db.CreateGame(appId, name, playtime);
						}
						await db.CreatePlayerGames(appId, discordId, playtime, playtime_windows, playtime_mac, playtime_linux, playtime_deck);
					}));
				}
			}
		}

		//method for updating game where first we subtract time from games previous steam user had and then do basically AddGame (used when user updates steamId)
		public async Task UpdateGames(string discordId, long steamId, long oldSteamId)
		{
			Database db = Database.Instance;
			var root = await stm.GetGames(Convert.ToString(oldSteamId));
			
			var tasks = new List<Task>();
			//erro handling to basically validate old id
			if(Convert.ToString(root) != "")
			{
				var games = root.EnumerateArray();

				while (games.MoveNext())
				{
					var game = games.Current;

					if (game.GetProperty("playtime_forever").GetInt32() != 0)
					{
						tasks.Add(Task.Run(async () =>
						{
							int appId = game.GetProperty("appid").GetInt32();
							int playtime = game.GetProperty("playtime_forever").GetInt32();

							if (await db.GetGame(appId) == "1")
							{
								await db.UpdateGameTimeSub(appId, playtime);
							}
						}));
					}
				}
			}
			tasks.Add(Task.Run(async () => {await db.DeletePlayerGames(discordId);}));
						
			await Task.WhenAll(tasks);
			await AddGames(discordId, steamId);  
		}
	}
}
