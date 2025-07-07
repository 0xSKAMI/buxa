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
		public static Game game = new Game();

		//method to handle /connect and update database
		public async Task ConnectPlayer(string discordId, long steamId)
		{
			Database db = Database.Instance;
			var reader = await db.GetUser(discordId);
			if (reader != Convert.ToString(steamId) && reader != "0" && reader != "3")
			{
				game.UpdateGames(steamId, long.Parse(reader));

				await db.UpdateUser(discordId, steamId);
			}
			else if(reader == "0")
			{
				game.AddGames(steamId);

				await db.CreateUser(discordId, steamId);
			}
		}
	}
}
