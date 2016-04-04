using System;
using SQLite;
using Java.IO;
using System.Runtime.Remoting.Contexts;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Gms.Maps;
using System;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Map
{
	public class RepositoryDB
	{
		public string PathDataBase{ get; private set;}
		SQLiteConnection database;
		public RepositoryDB ()
		{
			//var docsFolder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			string docsFolder="/sdcard";
			PathDataBase = System.IO.Path.Combine (docsFolder,"DB.sqlite");
			database = new SQLiteConnection (PathDataBase);
			database.CreateTable<Bus> ();
			database.CreateTable<Stop> ();
			database.CreateTable<StopTime>();
		}
		public List<T> GetItems<T>() where T : new()
		{
			return (from i in database.Table<T> ()select i).ToList ();
		}

		public void UpdateAll<T>(List<T> items) where T :new()
		{
			database.UpdateAll (items);
		}
	}
}

