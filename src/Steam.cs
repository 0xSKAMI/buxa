using System.Net.Http; //provide Http related classes
using System; //provodes sustem classes such as Console
using DotNetEnv; // To load environment variables from a `.env` file
using System.Text.Json;

//creating Steam namespace
namespace SteamN{
		public class Steam
		{
			//initialise http client
			public static HttpClient Client = new HttpClient();
			//this method is used to connect to server and get data
			public async void Connect()
			{
				//load env variables
				Env.Load();
				//get STEAM_KEY from .env file
				string steam_key = Env.GetString("STEAM_KEY");	
			
				HttpResponseMessage result = new HttpResponseMessage();
				while (true)
				{
					//send GET request to server
					result = await Client.GetAsync($"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={steam_key}&steamid=76561199571191880&include_appinfo=true&include_played_free_games=true&include_free_sub=true&skip_unvetted_apps=true&include_extended_appinfo=true");
					if (result.IsSuccessStatusCode) break;
				}
				string content = await result.Content.ReadAsStringAsync();
				string jsonString = JsonSerializer.Serialize(content);
			}
		}
}
