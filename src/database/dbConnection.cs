using DotNetEnv;
using Npgsql;

// here is code for the database
namespace Database
{
	// this is database class that conatins all of the db methods right now
	public class DB
	{
		//this is database connection object
		NpgsqlConnection globalConn;
		
		//this is connection string used for connecting database
		private readonly string _connString;
		
		//this is constructor method that defines _connString or connection string
		public DB()
		{
			Env.Load();
			string connString = Env.GetString("DB");
			_connString = connString;
			
			//giving globalConn value of connection
			globalConn = new NpgsqlConnection(connString);
		}

		public async Task AddPlayer(string playerId)
		{
			await globalConn.OpenAsync();

			using var cmd = new NpgsqlCommand();
			cmd.CommandText = $"INSERT INTO Players(PlayerId) VALUES(@Id)";
			cmd.Parameters.AddWithValue("Id", playerId);
			cmd.Connection = globalConn;


			await cmd.ExecuteNonQueryAsync();

			await CloseDb();
		}

		public async Task AddGame(string gameName)
		{
			await globalConn.OpenAsync();

			using var cmd = new NpgsqlCommand();
			cmd.CommandText = $"INSERT INTO Games(Name) VALUES(@name)";
			cmd.Parameters.AddWithValue("name", gameName);
			cmd.Connection = globalConn;


			await cmd.ExecuteNonQueryAsync();

			await CloseDb();
		
		}

		public async Task AddPlayerGame()
		{
			await globalConn.OpenAsync();

			using var cmd = new NpgsqlCommand();
			cmd.CommandText = "SELECT * FROM Games";
			cmd.Connection = globalConn;
	
			using var result = await cmd.ExecuteReaderAsync();


			while(await result.ReadAsync())
			{
				Console.WriteLine(result.GetString(1));
			}

			await CloseDb();
		}

		public async Task FindPlayer(string playerId)
		{
			await globalConn.OpenAsync();

			using var cmd = new NpgsqlCommand();
			cmd.CommandText = "SELECT * FROM Players";
			cmd.Parameters.AddWithValue("id", playerId);
			cmd.Connection = globalConn;
			
			using var result = await cmd.ExecuteReaderAsync();

			while(await result.ReadAsync())
			{
				Console.WriteLine(result.GetString(1));
			}
			
			await CloseDb();
		}
	
		//this method is used for closing connection with db (database)
		private static async Task CloseDb()
		{
			DB Database = new DB();

			await Database.globalConn.CloseAsync();
		}
	}
}
