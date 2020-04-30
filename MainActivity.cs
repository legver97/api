using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Google.Places;
using System;
using System.Collections.Generic;

namespace Map_L4
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        MapFragment myMapView;
        //установка места по умолчанию: Владивосток
        LatLng Vladik = new LatLng(43.116667, 131.900000);
        GoogleMap myMap;
        Button searchButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // загрузка карты
            myMapView = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            myMapView.GetMapAsync(this);
            //кнопка поиска
            searchButton = (Button)FindViewById<Button>(Resource.Id.btnOpenSearch);
            searchButton.Click += BtnOpenSearch_Click;
            if (!PlacesApi.IsInitialized)
            {
                // запуск поиска места
                PlacesApi.Initialize(this, "AIzaSyBPZtIau36YyBHlcA9doKwiSBp0hqpdf_U");
            }
        }

        private void BtnOpenSearch_Click(object sender, EventArgs e)
        {
            // список значений для поиска места
            List<Place.Field> fields = new List<Place.Field>();
            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);

            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .Build(this);

            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                var place = Autocomplete.GetPlaceFromIntent(data);
                if (place == null)
                {
                    return;
                }

                // установка маркера на найденном месте
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(place.LatLng);
                markerOptions.SetTitle(place.Name);
                myMap.AddMarker(markerOptions);
                myMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            // когда карта загружена
            myMap = googleMap;
            MarkerOptions markerOptions = new MarkerOptions();
            // установка позиции: Владивосток
            markerOptions.SetPosition(Vladik);
            markerOptions.SetTitle("Vladivostok");
            myMap.AddMarker(markerOptions);
            myMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(Vladik, 5));
        }
    }
}