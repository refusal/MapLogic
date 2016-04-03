using System;
using SQLite;
namespace Map
{
	public class StopTime
	{
		[PrimaryKey]
		public int ID{ get; set;}
		public int StopID{ get; set;}
		public string Time{ get; set;}
		public int BusID{ get; set;}
	}
}

