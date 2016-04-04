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
using System.IO;
using Java.IO;


namespace Map
{
	[Activity (Label = "Map", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity , ILocationListener
	{
		public static List<Stop> Stops;
		public static List<StopTime> StopTimes;
		public static List<Bus> Buses;
		GoogleMap map;
		Location currentLoc;
		LocationManager locationManager;
		string locationProvider;
		Android.Gms.Maps.Model.Polyline currentLine;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);
			RepositoryDB DB = new RepositoryDB ();
			CopyDataBase (DB.PathDataBase);
			Stops = DB.GetItems<Stop> ();
			Buses = DB.GetItems<Bus> ();
			StopTimes = DB.GetItems<StopTime> ();
			SetUpLocManager ();
			SetUpMap ();

		}
		private void PutMarkers()
		{
			foreach (var stop in Stops) {
				var marker = new MarkerOptions ();
				marker.SetTitle (stop.Name+" ID "+stop.ID.ToString());
				marker.SetPosition (new LatLng (stop.Latitude, stop.Longitude));
				marker.SetIcon (BitmapDescriptorFactory.FromResource(Resource.Drawable.bus));
				map.AddMarker (marker);
			}
		}
		private void SetUpMap ()
		{
			var frag = FragmentManager.FindFragmentById<MapFragment> (Resource.Id.map); 
			var mapReadyCallback = new OnMapReadyClass (); 
			mapReadyCallback.MapReadyAction += delegate(GoogleMap obj) {
				map = obj;  
				SetUpMapSettings ();
			};  
			frag.GetMapAsync (mapReadyCallback);     

		}
		private void SetUpLocManager()
		{
			locationManager = (LocationManager) GetSystemService(LocationService);

			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Coarse
			};
//			IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);
//			if (acceptableLocationProviders.Any())
//			{
//				locationProvider = acceptableLocationProviders.First();
//			}
			locationProvider=locationManager.GetBestProvider(criteriaForLocationService,true);
				
				
		}
		private  void SetUpMapSettings ()
		{
			locationManager.RequestLocationUpdates(locationProvider, 2000,1, this);
			map.MyLocationEnabled = true;
			map.UiSettings.MyLocationButtonEnabled = true;
			map.UiSettings.ZoomControlsEnabled = true;
			PutMarkers ();
			List<Stop> fff = new List<Stop> ();
			fff.Add (Stops [0]);
			fff.Add (Stops [2]);
			fff.Add (Stops [40]);
			fff.Add (Stops [18]);
			fff.Add (Stops [12]);
			fff.Add (Stops [28]);


			DrawPath (fff);
		}
		public void OnLocationChanged (Android.Locations.Location location)
		{
			if(currentLoc==null)
			 map.AnimateCamera (CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude,location.Longitude),15f));
			currentLoc = location;
		}
		public void OnProviderDisabled (string provider)
		{
		}
		public void OnProviderEnabled (string provider)
		{
		}
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}
		async public void DrawPath (List<Stop> stops)
		{
			string strFullDirectionURL = await HttpConnection.CreateUri (stops);
			string strJSONDirectionResponse = await HttpConnection.HttpRequest (strFullDirectionURL);
			if (strJSONDirectionResponse != "") {
				RunOnUiThread (() => { 
					if (currentLine != null) { 
						currentLine=null;
					}
				});
				var latLngPoints = await JSonParse.SetDirectionQuery (strJSONDirectionResponse);
				var polylineoption = new PolylineOptions (); 
				polylineoption.InvokeColor (Android.Graphics.Color.BlueViolet);
				polylineoption.Geodesic (true);
				polylineoption.Add (latLngPoints); 
				polylineoption.InvokeWidth (5);
				currentLine = map.AddPolyline (polylineoption);
			} else {
					Toast.MakeText (this, "Невозможно проложить маршрут", ToastLength.Short).Show (); 
			}	
		}
		protected override void OnResume()
		{
			base.OnResume();
		}
		protected override void OnPause()
		{
			base.OnPause();
			locationManager.RemoveUpdates(this);

		}
		public void CopyDataBase(string path)
		{
			byte[] buffer = new byte[2048];
			Stream myInput = null;
			int length=2048;
			OutputStream myOutput = null;
			try
			{
				myInput =Assets.Open("DB.sqlite");
				myOutput =new FileOutputStream(path);
				while((length = myInput.Read(buffer,0,length)) > 0)
				{
					myOutput.Write(buffer, 0, length);
				}
				myOutput.Close();
				myOutput.Flush();
				myInput.Close();
			}
			catch
			{
			}
		}

	}

	public class OnMapReadyClass :Java.Lang.Object,IOnMapReadyCallback
	{
		public GoogleMap Map { get; private set; }
		public event Action<GoogleMap> MapReadyAction;
		public void OnMapReady (GoogleMap googleMap)
		{
			Map = googleMap;
			if (MapReadyAction != null)
				MapReadyAction (Map);
		}
	}
}


