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
	[Activity (Label = "Map", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity , ILocationListener
	{
		GoogleMap map;
		Location currentLoc;
		LocationManager locationManager;
		string locationProvider;
		Android.Gms.Maps.Model.Polyline currentLine;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);
			SetUpLocManager ();
			SetUpMap ();

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
			DrawPath ();
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

		async public void DrawPath ()
		{
			string strFullDirectionURL = await HttpConnection.CreateUri ();
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
				RunOnUiThread (() =>
					Toast.MakeText (this, "Невозможно проложить маршрут", ToastLength.Short).Show ()); 
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


