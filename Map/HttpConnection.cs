using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Map
{
	public class HttpConnection
	{
		public static async Task<string> HttpRequest(string strUri)
		{ 
			WebClient webclient = new WebClient ();
			string strResultData;
			try
			{
				strResultData= await webclient.DownloadStringTaskAsync (new Uri(strUri));
			}
			catch
			{
				strResultData = "";
			}
			finally
			{
				if ( webclient!=null )
				{
					webclient.Dispose ();
					webclient = null; 
				}
			}

			return strResultData;
		}

		public static Task<string> CreateUri(List<Stop> stops)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo ("en-US");
			string origin =  string.Format("{0},{1}",  stops[0].Latitude,stops[0].Longitude);
			stops.Remove (stops [0]);
			string dest =  string.Format("{0},{1}",stops [stops.Count - 1].Latitude,stops [stops.Count - 1].Longitude);
			stops.Remove (stops [stops.Count - 1]);
			string waypoints="";
			foreach (var stop in stops) {
				waypoints=waypoints+stop.Latitude+","+stop.Longitude+"|";
			}


			string uri = "https://maps.googleapis.com/maps/api/directions/json?origin="+origin+"&destination="+dest+"&waypoints="+waypoints+"&key=AIzaSyDMMNH7--IC5XdyCtF4oRvIIQgWKs4RHb0";

			return Task<string>.Factory.StartNew(()=>uri);
		}

	}
}


