using System;
using SQLite;

namespace Map
{
	public class Stop
	{
		[PrimaryKey]
		public int ID{ get; set;}
		public string Name{ get; set;}
		public double Latitude{ get; set;}
		public double Longitude{ get; set;}
		public string Lines{ get; set;}

	}
}

