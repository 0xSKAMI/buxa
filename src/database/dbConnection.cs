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

		public async Task AddPlayerGame(int gameId, string playerId, TimeSpan date)
		{
			await globalConn.OpenAsync();
		
			using var cmd = new NpgsqlCommand();
			cmd.CommandText = "INSERT INTO PlayerGames(GameId, PlayerId, played_time) VALUES(@gid, @pid, @date)";
			cmd.Parameters.AddWithValue("gid", gameId);
			cmd.Parameters.AddWithValue("pid", playerId);
			cmd.Parameters.AddWithValue("date", date);
			cmd.Connection = globalConn;
	
			await cmd.ExecuteNonQueryAsync();

			await CloseDb();
		 }

		public async Task FindPlayer(string playerId)
		{
			await globalConn.OpenAsync();

			using var cmd = new NpgsqlCommand();
			cmd.CommandText = $"SELECT * FROM Players WHERE PlayerId = @id";
			cmd.Parameters.AddWithValue("id", playerId);
			cmd.Connection = globalConn;
			
			using var result = await cmd.ExecuteReaderAsync();
		 
			Console.WriteLine(result.HasRows);

			await CloseDb();
		}

		public async Task FindGame(string gameName)
		{
			await globalConn.OpenAsync();

			using var cmd = new NpgsqlCommand();
			cmd.CommandText = $"SELECT * FROM Games WHERE name = @name";
			cmd.Parameters.AddWithValue("name", gameName);
			cmd.Connection = globalConn;
			
			using var result = await cmd.ExecuteReaderAsync();
		 
			Console.WriteLine(result.HasRows);

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
