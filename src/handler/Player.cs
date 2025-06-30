using DB;
using System.Data.Common;


//namespace for all handlers
namespace Handler
{
	//Player class where every player method will be written
	public class Player
	{
		//method to handle /connect and update database
		public async Task ConnectPlayer(string discordId, long steamId)
		{
			Database db = Database.Instance;
			var reader = await db.GetUser(discordId);
			if (reader == 1)
			{
				await db.UpdateUser(discordId, steamId);
			}
			else
			{
				await db.CreateUser(discordId, steamId);
			}
		}
	}
}
