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
		public async Task AddGames(long steamId)
		{
			//creating database instance
			Database db = Database.Instance;
	
			//get user's games in steam library
			var root = await stm.GetGames(Convert.ToString(steamId));
			var games = root.EnumerateArray();
			//make a task so it will run in background
			_= Task.Run(async () => {
				//start a loop
				while(games.MoveNext())
					{
						if(Convert.ToString(games.Current.GetProperty("playtime_forever")) != "0")
						{
							if (await db.GetGame(games.Current.GetProperty("appid").GetInt32()) == "1")
							{
								await db.UpdateGameTimeAdd(games.Current.GetProperty("appid").GetInt32(), games.Current.GetProperty("playtime_forever").GetInt32());
							}
							else
							{
								await db.CreateGame(games.Current.GetProperty("appid").GetInt32(), games.Current.GetProperty("name").GetString(), games.Current.GetProperty("playtime_forever").GetInt32());
							}
						}
					}
				}
			);
		}

		//method for updating game where first we subtract time from games previous steam user had and then do basically AddGame (used when user updates steamId)
		public async Task UpdateGames(long steamId, long oldSteamId)
		{
			Database db = Database.Instance;

			var root = await stm.GetGames(Convert.ToString(steamId));
			var games = root.EnumerateArray();
			//declare task and give it name so we can wait for this task to finish
			Task subtract = Task.Run(async () => {
				while(games.MoveNext())
					{
						if(Convert.ToString(games.Current.GetProperty("playtime_forever")) != "0")
						{
							if (await db.GetGame(games.Current.GetProperty("appid").GetInt32()) == "1")
							{
								Console.WriteLine("sab");
								await db.UpdateGameTimeSub(games.Current.GetProperty("appid").GetInt32(), games.Current.GetProperty("playtime_forever").GetInt32());
							}
						}
					}
				}
			);
			subtract.Wait();
			AddGames(steamId);
		}
	}
}
