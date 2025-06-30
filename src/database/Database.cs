using DotNetEnv;
using Npgsql;


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
		public async Task CreateUser(string discordId, int steamId)
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

		public async Task UpdateUser(string discordId, string steamId)
		{
		}

		public async Task GetUser(string discordId, string steamId)
		{
		}

		public async Task DeleteUser(string discordId, string steamId)
		{
		}

		public async Task CreateGame(string discordId, string steamId)
		{
		}

		public async Task Deletegame(string discordId, string steamId)
		{
		}

		public async Task UpdateGame(string discordId, string steamId)
		{
		}

		public async ValueTask DisposeAsync()
		{
			await dataSource.DisposeAsync();
		}
	}
}
