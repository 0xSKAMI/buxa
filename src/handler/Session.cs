using DB;
using System.Data.Common;
using SteamN;
using System.Text.Json;


namespace Handler
{
	public class Session
	{
		//initilize steam object
		public static Steam stm = new Steam();

		public async Task CreateSessions(string discordId, string steamId)
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
						int[] playtime = await db.GetPlayerGames(game.GetProperty("appid").GetInt32(), discordId);

						if(game.GetProperty("playtime_forever").GetInt16() > playtime[0])
						{
							int appId = game.GetProperty("appid").GetInt32();
							int playtime_full = game.GetProperty("playtime_forever").GetInt32() - playtime[0];
							int playtime_windows = game.GetProperty("playtime_windows_forever").GetInt32() - playtime[1];
							int playtime_mac = game.GetProperty("playtime_mac_forever").GetInt32() - playtime[2];
							int playtime_linux = game.GetProperty("playtime_linux_forever").GetInt32() - playtime[3];
							int playtime_deck = game.GetProperty("playtime_deck_forever").GetInt32() - playtime[4];
							string name = game.GetProperty("name").GetString();

							await db.CreateSession(appId, discordId, playtime_full, playtime_windows, playtime_mac, playtime_linux, playtime_deck);
						}
					}));
				}
			}
		}

		public async Task SessionScheduler()
		{
			//creating database instance
			Database db = Database.Instance;

			while(true)
			{
				TimeSpan some = DateTime.Now.AddSeconds(3) - DateTime.Now;

				Dictionary<string, string> users = new Dictionary<string, string>();

				users = await db.GetAllUsers();
				
				foreach(var user in users)
				{
					await CreateSessions(user.Key, user.Value);
				}
				await Task.Delay(TimeSpan.FromHours(12));
			}
		}
	}
}
