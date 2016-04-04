using System;
using Newtonsoft.Json;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Collections.Generic;
using Android.Widget;
using System.Threading.Tasks;

namespace Map
{
	public class JSonParse
	{

		public static Task<LatLng[]> SetDirectionQuery(string strJSONDirectionResponse)
		{
			var objRoutes = JsonConvert.DeserializeObject<GoogleDirectionClass> ( strJSONDirectionResponse );  
			if ( objRoutes.routes.Count > 0 )
			{
				string encodedPoints =	objRoutes.routes [0].overview_polyline.points; 
				List<Location> lstDecodedPoints = DecodePolylinePoints ( encodedPoints ); 
				//convert list of location point to array of latlng type
				var latLngPoints = new LatLng[lstDecodedPoints.Count]; 
				int index = 0;
				foreach ( Location loc in lstDecodedPoints )
				{
					latLngPoints [index++] = new LatLng ( loc.Latitude , loc.Longitude );
				}
				return  Task<LatLng[]>.Factory.StartNew(()=>latLngPoints);
			}
			return null;
		}
		private static List<Location> DecodePolylinePoints(string encodedPoints) 
		{
			if ( string.IsNullOrEmpty ( encodedPoints ) )
				return null;
			var poly = new List<Location>();
			char[] polylinechars = encodedPoints.ToCharArray();
			int index = 0;

			int currentLat = 0;
			int currentLng = 0;
			int next5bits;
			int sum;
			int shifter;

			try
			{
				while (index < polylinechars.Length)
				{
					sum = 0;
					shifter = 0;
					do
					{
						next5bits = (int)polylinechars[index++] - 63;
						sum |= (next5bits & 31) << shifter;
						shifter += 5;
					} while (next5bits >= 32 && index < polylinechars.Length);

					if (index >= polylinechars.Length)
						break;

					currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

					sum = 0;
					shifter = 0;
					do
					{
						next5bits = (int)polylinechars[index++] - 63;
						sum |= (next5bits & 31) << shifter;
						shifter += 5;
					} while (next5bits >= 32 && index < polylinechars.Length);

					if (index >= polylinechars.Length && next5bits >= 32)
						break;

					currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
					Location p = new Location("");
					p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
					p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
					poly.Add(p);
				} 
			}
			catch 
			{
				
			}
			return poly;
		}

	}
}

