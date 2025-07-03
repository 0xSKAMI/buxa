using DB;
using System.Data.Common;
using SteamN;
using System.Text.Json;

//namespace for all handlers
namespace Handler
{
	//Player class where every player method will be written
	public class Player
	{
		public static Steam stm = new Steam();

		//method to handle /connect and update database
		public async Task ConnectPlayer(string discordId, long steamId)
		{
			Database db = Database.Instance;
			var reader = await db.GetUser(discordId);
			if (reader != Convert.ToString(steamId) && reader != "0" && reader != "3")
			{
				var root = await stm.GetGames(Convert.ToString(steamId));
				var games = root.EnumerateArray();
				_= Task.Run(async () => {
					while(games.MoveNext())
					{
						if(Convert.ToString(games.Current.GetProperty("playtime_forever")) != "0")
						{
							await db.CreateGame(Convert.ToInt32(games.Current.GetProperty("appid")), Convert.ToString(games.Current.GetProperty("name")), Convert.ToInt32(games.Current.GetProperty("playtime_forever")));
						}
					}
				});
				await db.UpdateUser(discordId, steamId);
			}
			else if(reader == "0")
			{
				var root = await stm.GetGames(Convert.ToString(steamId));
				var games = root.EnumerateArray();
				var gameProcess = Task.Run(async () => {
					while(games.MoveNext())
					{
						if(Convert.ToString(games.Current.GetProperty("playtime_forever")) != "0")
						{
							await db.CreateGame(Convert.ToInt32(games.Current.GetProperty("appid")), Convert.ToString(games.Current.GetProperty("name")), Convert.ToInt32(games.Current.GetProperty("playtime_forever")));
						}
					}
				});
				await db.CreateUser(discordId, steamId);
			}
		}
	}
}
