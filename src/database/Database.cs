using DotNetEnv;
using Npgsql;
using System.Data.Common;


//namespace for all of database classes
namespace DB 
{
	//Class where basic methods will be written
	public sealed class Database : IAsyncDisposable
	{
		private static readonly Lazy<Database> lazyInstance = new(() => new Database());
		public static Database Instance => lazyInstance.Value;

		private static NpgsqlDataSource dataSource;

		//This constructor connects to database
		private Database()
		{
			//loading connection variable from .env and connect to database
			Env.Load();
			string _connString = Env.GetString("DB");
			dataSource = NpgsqlDataSource.Create(_connString);
		}
	
		//method to craerte user
		public async Task CreateUser(string discordId, long steamId)
		{
			await using var command = dataSource.CreateCommand("INSERT INTO players (playerid, steamid) VALUES ($1, $2)");
			command.Parameters.AddWithValue(discordId);
			command.Parameters.AddWithValue(steamId);

			try
			{
				await command.ExecuteNonQueryAsync();
			}
			catch 
			{
				Console.WriteLine("some error happened");
			}
		}

		//method to update user (in case user wants to change steam account)
		public async Task UpdateUser(string discordId, long steamId)
		{
			await using var command = dataSource.CreateCommand("UPDATE players SET steamid = $2 WHERE playerid = $1");
			command.Parameters.AddWithValue(discordId);
			command.Parameters.AddWithValue(steamId);

			try
			{
				await command.ExecuteNonQueryAsync();
			}
			catch 
			{
				Console.WriteLine("some error happened");
				throw;
			}
		}
	
		//method to get users information
		public async Task<string> GetUser(string discordId)
		{
			await using var command = dataSource.CreateCommand("SELECT * FROM players WHERE playerid = ($1)");
			command.Parameters.AddWithValue(discordId);
			try
			{
				await using var reader = await command.ExecuteReaderAsync(); 

				string result = (reader.HasRows) ? "1" : "0";
				while (await reader.ReadAsync())
				{
					result = Convert.ToString(reader.GetValue(1));
				}
				return result;
			}
			catch 
			{
				throw;
			}
			return "3";
		}

		public async Task<Dictionary<string, string>> GetAllUsers()
		{
			await using var command = dataSource.CreateCommand("SELECT * FROM players");

			Dictionary<string, string> result = new Dictionary<string, string>();
			try
			{
				await using var reader = await command.ExecuteReaderAsync(); 

				while (await reader.ReadAsync())
				{
					result.Add(reader.GetString(0), Convert.ToString(reader.GetInt64(1)));

					string founder = reader.GetString(0);
				}
				return result;
			}
			catch 
			{
				throw;
			}
			return result;
		}

		public async Task<string> GetGame(int id)
		{
			await using var command = dataSource.CreateCommand("SELECT gameid FROM games WHERE gameid = $1");
			command.Parameters.AddWithValue(id);
			string result = "3";

			try
			{
				using var reader = command.ExecuteReader();

				result = (reader.HasRows == true) ? "1" : "0";
				return result;
			}
			catch
			{
				return "3";
				throw;
			}
		}

		public async Task CreateGame(int id, string name, int full)
		{
			await using var command = dataSource.CreateCommand("INSERT INTO games (gameid, name, full_time) VALUES ($1, $2, $3)");

			command.Parameters.AddWithValue(id);
			command.Parameters.AddWithValue(name);
			command.Parameters.AddWithValue(full);

			try
			{
				command.ExecuteNonQuery();
				await Task.CompletedTask;
			}
			catch
			{
				throw; 
			}
		}

		public async Task UpdateGameTimeAdd(int id, int time)
		{
			await using var command = dataSource.CreateCommand("UPDATE games SET full_time = full_time + $2 WHERE gameid = $1");
			command.Parameters.AddWithValue(id);
			command.Parameters.AddWithValue(time);
			
			try
			{
				command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
		}

		public async Task UpdateGameTimeSub(int id, int time)
		{
			await using var command = dataSource.CreateCommand("UPDATE games SET full_time = full_time - $2 WHERE gameid = $1");
			command.Parameters.AddWithValue(id);
			command.Parameters.AddWithValue(time);
			
			try
			{
				command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
		}

		public async Task<int[]> GetPlayerGames(int gameId, string playerId)
		{
			await using var command = dataSource.CreateCommand("SELECT * FROM playergames WHERE gameid = $1 AND playerid = $2");

			command.Parameters.AddWithValue(gameId);
			command.Parameters.AddWithValue(playerId);

			try
			{
				await using var reader = command.ExecuteReader();

				int[] result = [];
				while (await reader.ReadAsync())
				{
					result = new int[] {reader.GetInt16(2), reader.GetInt16(3), reader.GetInt16(4), reader.GetInt16(5), reader.GetInt16(6)};
				}

				return result;
			}
			catch
			{
				throw;
			}
		}
		
		public async Task CreatePlayerGames(int gameId, string playerId, int time, int time_windows, int time_mac, int time_linux, int time_deck)
		{
			await using var command = dataSource.CreateCommand("INSERT INTO playergames (gameid, playerid, played_time, windows_played, mac_played, linux_played, deck_played) VALUES ($1, $2, $3, $4, $5, $6, $7)");

			command.Parameters.AddWithValue(gameId);
			command.Parameters.AddWithValue(playerId);
			command.Parameters.AddWithValue(time);
			command.Parameters.AddWithValue(time_windows);
			command.Parameters.AddWithValue(time_mac);
			command.Parameters.AddWithValue(time_linux);
			command.Parameters.AddWithValue(time_deck);

			try
			{
				command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
		}

		public async Task UpdatePlayerGames(int gameId, string playerId, int time, int time_windows, int time_mac, int time_linux, int time_deck)
		{
			await using var command = dataSource.CreateCommand("UPDATE playergames SET played_time = played_time + $3, windows_played = windows_played + $4, mac_played = mac_played + $5, linux_played = linux_played + $6, deck_played = deck_played + $7 WHERE gameid = $1 AND playerid = $2");

			command.Parameters.AddWithValue(gameId);
			command.Parameters.AddWithValue(playerId);
			command.Parameters.AddWithValue(time);
			command.Parameters.AddWithValue(time_windows);
			command.Parameters.AddWithValue(time_mac);
			command.Parameters.AddWithValue(time_linux);
			command.Parameters.AddWithValue(time_deck);

			try
			{
				await command.ExecuteNonQueryAsync();
			}
			catch
			{
				throw;
			}

		}

		public async Task DeletePlayerGames(string playerId)
		{
			await using var command = dataSource.CreateCommand("DELETE FROM playergames WHERE playerid = $1");

			command.Parameters.AddWithValue(playerId);

			try
			{
				command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
		}

		public async Task<List<Array>> GetSessions(string id, string determiner, TimeSpan time)
		{
			await using var command = (determiner == "1") ? dataSource.CreateCommand("SELECT g.name AS game_name, SUM(s.played_time) AS total_played_time FROM sessions AS s JOIN games AS g ON s.gameid = g.gameid WHERE s.playerid = $1 AND s.creation_date >= NOW() - $2 GROUP BY g.name ORDER BY total_played_time DESC LIMIT 5") : dataSource.CreateCommand("SELECT g.name AS game_name, SUM(s.played_time) AS total_played_time FROM sessions AS s JOIN games AS g ON s.gameid = g.gameid WHERE s.playerid = $1 AND s.creation_date >= NOW() - INTERVAL '$2' GROUP BY g.name ORDER BY total_played_time ASC LIMIT 5");

			command.Parameters.AddWithValue(id);
			command.Parameters.AddWithValue(time);

			List<Array> result = new List<Array>();
			await using var reader = await command.ExecuteReaderAsync();
			while(await reader.ReadAsync())
			{
				string[] top = new string[] {Convert.ToString(reader.GetValue(0)), Convert.ToString(reader.GetValue(1))};
				result.Add(top);
			}

			return result;
		}

		public async Task CreateSession(int id, string playerId, int time, int windows_time, int mac_time, int linux_time, int deck_time)
		{
			await using var command = dataSource.CreateCommand("INSERT INTO sessions (gameid, playerid, played_time, windows_played, mac_played, linux_played, deck_played) VALUES ($1, $2, $3, $4, $5, $6, $7)");

			command.Parameters.AddWithValue(id);
			command.Parameters.AddWithValue(playerId);
			command.Parameters.AddWithValue(time);
			command.Parameters.AddWithValue(windows_time);
			command.Parameters.AddWithValue(mac_time);
			command.Parameters.AddWithValue(linux_time);
			command.Parameters.AddWithValue(deck_time);

			try
			{
				command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
		}

		public async ValueTask DisposeAsync()
		{
			await dataSource.DisposeAsync();
		}
	}
}
