using System;
using System.Net;
using System.Threading.Tasks;

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

		public static Task<string> CreateUri()
		{
			string a = "https://maps.googleapis.com/maps/api/directions/json?origin=53.934446,30.252693&destination=53.834075,30.360496&waypoints=53.929999,30.352256|53.929999,30.352250&key=AIzaSyDMMNH7--IC5XdyCtF4oRvIIQgWKs4RHb0";

			return Task<string>.Factory.StartNew(()=>a);
		}

	}
}

