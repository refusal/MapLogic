using System;
using SQLite;

namespace Map
{
	public class Bus
	{
		[PrimaryKey]
		public int ID{ get; set;}
		public string Number{ get; set;}
		public string Source{ get; set;}
		public string Destination{ get; set;}
		public string OperateData{ get; set;}
		public string Stops{ get; set;}

	}
}

