using DotNetEnv;
using Npgsql;


namespace Database
{
	public class DB
	{
		public async Task ConnectDB()
		{
			Env.Load();
			string connString = Env.GetString("DB");
			string connectionString = connString;

			await using var conn = new NpgsqlConnection(connectionString);

			await conn.OpenAsync();
		}

		public async Task CloseDB()
		{
			Console.WriteLine("this is for closing connection with database");
		}
	}
}
