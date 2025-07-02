using System.Net.Http; //provide Http related classes
using System; //provodes sustem classes such as Console
using DotNetEnv; // To load environment variables from a `.env` file
using System.Text.Json;
using System.Net;
using System.Web;

//creating Steam namespace
namespace SteamN{
		public class Steam
		{
			//initialise http client
			public static HttpClient Client = new HttpClient();
			//this method is used to connect to server and get data
			public async Task<JsonElement> GetGames(string steamId)
			{
				//load env variables
				Env.Load();
				//get STEAM_KEY from .env file
				string steam_key = Env.GetString("STEAM_KEY");	
			
				HttpResponseMessage result = new HttpResponseMessage();
				while (true)
				{
					//send GET request to server
					result = await Client.GetAsync($"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={steam_key}&steamid={steamId}&include_appinfo=true&include_played_free_games=true&include_free_sub=true&skip_unvetted_apps=true&include_extended_appinfo=true");
					if (result.IsSuccessStatusCode) break;
				}
				string content = await result.Content.ReadAsStringAsync();
				var jsonString = JsonDocument.Parse(content);
				var root = jsonString.RootElement.GetProperty("response").GetProperty("games");
				return root;
			}
	
			//method to listen to port
			public static async Task<string> ListenToPort()
			{
				//initilise listener
				HttpListener listener = new HttpListener();
				//give it prefix to listen on
				listener.Prefixes.Add("http://127.0.0.1:3000/return/");
				string steamId = "";
				try
				{
					listener.Start();

					//get context (object that contains practically everything we need)
					HttpListenerContext context = listener.GetContext();
					//get request to extract steamId
					HttpListenerRequest request = context.Request;
					var paramsCollection = HttpUtility.ParseQueryString(request.Url.Query);
					steamId = paramsCollection["openid.claimed_id"].Split('/').Last();
					if (string.IsNullOrEmpty(steamId))
					{
						throw new Exception("Steam login failed or was cancelled. No claimed_id returned.");
					}
					
					//get response (we will need to send HTML to user)
					HttpListenerResponse response = context.Response;
					RespondHtml(response, "<HTML><BODY>you logged in successfully !</BODY></HTML>");
				}
				catch 
				{
					listener.Start();

					//get context (object that contains practically everything we need)
					HttpListenerContext context = listener.GetContext();

					//get response (we will need to send HTML to user)
					HttpListenerResponse response = context.Response;
					RespondHtml(response, "<HTML><BODY>something went wront, try again</BODY></HTML>");
				}
				finally
				{
					listener.Stop();
				}
				return steamId;
			}

			private static void RespondHtml(HttpListenerResponse response, string toSend)
			{
				//create HTML and turn it into stream of butes to send
				System.IO.Stream output = response.OutputStream;
				string responseString = toSend;
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				
				//send HTML
				output.Write(buffer,0,buffer.Length);
				// Close connection and stop listening 
				output.Close();
			}
		}
}
