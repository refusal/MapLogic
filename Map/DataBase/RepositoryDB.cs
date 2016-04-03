using System;
using SQLite;
namespace Map
{
	public class RepositoryDB
	{
		public RepositoryDB ()
		{
			var docsFolder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Resources);
			var pathDataBase = System.IO.Path.Combine (docsFolder,"DB.db");
			var connection = new SQLiteConnection (pathDataBase);
			connection.CreateTable<Bus> ();
			connection.CreateTable<Stop> ();
			connection.CreateTable<StopTime>();

		}
	}
}

