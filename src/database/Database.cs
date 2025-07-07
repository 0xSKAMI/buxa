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

		public async Task<string> GetGame(int id)
		{
			await using var command = dataSource.CreateCommand("SELECT gameid FROM games WHERE gameid = $1");
			command.Parameters.AddWithValue(id);
			string result = "3";

			try
			{
				await using var reader = await command.ExecuteReaderAsync();

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
				await command.ExecuteNonQueryAsync();
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
				await command.ExecuteNonQueryAsync();
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
				await command.ExecuteNonQueryAsync();
				Console.WriteLine(id);
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
