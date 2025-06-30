using DotNetEnv;
using Npgsql;


//namespace for all of database classes
namespace DB 
{
	//Class where basic methods will be written
	public class Database
	{
		//This method connects to database
		public async void Connect()
		{
			//loading connection variable from .env and connect to database
			Env.Load();

			string _connString = Env.GetString("DB");

			await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connString);
		}
	}
}
