﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using static Xamarin.Forms.Forms;

namespace LunchBoxApp.Droid
{
    [Activity(Label = "Lunch Box", 
        Icon = "@drawable/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

