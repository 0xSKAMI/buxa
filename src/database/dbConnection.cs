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
		
		//This method adds player to DB (players are unique)	
		public async Task AddPlayer(string playerId)
		{
			await globalConn.OpenAsync();

			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "INSERT INTO Players(PlayerId) VALUES(@Id)";
				cmd.Parameters.AddWithValue("Id", playerId);
				cmd.Connection = globalConn;

				await cmd.ExecuteNonQueryAsync();
			}
			finally
			{
				await globalConn.CloseAsync();
			}
		}

		//This method adds Games to DB (games are unique)
		public async Task AddGame(string gameName)
		{
			await globalConn.OpenAsync();

			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "INSERT INTO Games(Name) VALUES(@name)";
				cmd.Parameters.AddWithValue("name", gameName);
				cmd.Connection = globalConn;

				await cmd.ExecuteNonQueryAsync();
			}
			finally
			{
				await globalConn.CloseAsync();
			}
		}

		//This method adds Playergames to DB
		public async Task AddPlayerGame(int gameId, string playerId, TimeSpan time)
		{
			await globalConn.OpenAsync();
		
			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "INSERT INTO PlayerGames(gameid, playerid, played_time) VALUES(@gid, @pid, @time)";
				cmd.Parameters.AddWithValue("gid", gameId);
				cmd.Parameters.AddWithValue("pid", playerId);
				cmd.Parameters.AddWithValue("time", time);
				cmd.Connection = globalConn;
				
				await cmd.ExecuteNonQueryAsync();
			}
			finally
			{
				await globalConn.CloseAsync();
			}

		}

		//method to get info about top 5 games played in week
		public async Task<WeekGame[]> FindMostPlayedGameWeek()
		{
			await globalConn.OpenAsync();

			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "SELECT games.name, SUM(played_time) AS total_played_time FROM playergames INNER JOIN games ON playerGames.gameId = Games.gameid GROUP BY games.gameId ORDER BY total_played_time DESC LIMIT 5";

				cmd.Connection = globalConn;

				using var result = await cmd.ExecuteReaderAsync();
				
				WeekGame[] localWeekGame = new WeekGame[result.FieldCount];
				int i = 0;
				while(await result.ReadAsync())
				{
					localWeekGame[i] = new WeekGame {gameName = Convert.ToString(result.GetValue(0)), playTime = result.GetTimeSpan(1)};
					i++;
				}
				return localWeekGame;
			}
			finally
			{
				await globalConn.CloseAsync();
			}
		}

		//Returs bool based on existanse of player
		public async Task<bool> FindPlayer(string playerId)
		{
			await globalConn.OpenAsync();

			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "SELECT * FROM Players WHERE PlayerId = @id";
				cmd.Parameters.AddWithValue("id", playerId);
				cmd.Connection = globalConn;
				
				using var result = await cmd.ExecuteReaderAsync();

				return result.HasRows;
			}
			finally
			{
				await globalConn.CloseAsync();
			}
		}

		//This is method for finding if game exists or not
		public async Task<bool> FindGame(string gameName)
		{
			await globalConn.OpenAsync();
			
			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "SELECT * FROM Games WHERE name = @name";
				cmd.Parameters.AddWithValue("name", gameName);
				cmd.Connection = globalConn;
			
				using var result = await cmd.ExecuteReaderAsync();
		 
				return result.HasRows;
			}
			finally
			{
				await globalConn.CloseAsync();
			}
		}

		public async Task<int> FindGameId(string gameName)
		{
			await globalConn.OpenAsync();

			try
			{
				using var cmd = new NpgsqlCommand();
				cmd.CommandText = "SELECT GameId FROM Games WHERE name = @game";
				cmd.Parameters.AddWithValue("game", gameName);
				cmd.Connection = globalConn;

				using var result = await cmd.ExecuteReaderAsync();

				if(result.HasRows)
				{
					await result.ReadAsync();
					return Convert.ToInt32(result.GetInt32(0));
				}
				else
				{
					return -1;
				}
			}
			finally
			{
				await globalConn.CloseAsync();
			}
		}
	}

	public class WeekGame
	{
		public string gameName {get; set;}
		public TimeSpan playTime {get; set;}
	}
}
