using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.Maui;

namespace InventoryApp;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize
        | ConfigChanges.Orientation
        | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout
        | ConfigChanges.SmallestScreenSize
        | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const string StartupTag = "InventoryStartup";

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        Log.Info(StartupTag, "MainActivity.OnCreate: before MAUI initialization");
        try
        {
            base.OnCreate(savedInstanceState);
            Log.Info(StartupTag, "MainActivity.OnCreate: MAUI initialization completed");
        }
        catch (Exception ex)
        {
            Log.Error(StartupTag, "MainActivity failed during MAUI initialization: " + ex);
            ShowNativeStartupError(ex);
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        Log.Info(StartupTag, "MainActivity.OnResume");
    }

    private void ShowNativeStartupError(Exception exception)
    {
        try
        {
            var message = new TextView(this)
            {
                Text = "Inventory Management could not start.\n\n" + exception,
                TextSize = 14,
                Gravity = GravityFlags.Start
            };
            message.SetTextColor(Android.Graphics.Color.Red);
            message.SetBackgroundColor(Android.Graphics.Color.White);
            message.SetPadding(32, 32, 32, 32);

            var scroll = new ScrollView(this) { FillViewport = true };
            scroll.AddView(message);
            SetContentView(scroll, new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent));
        }
        catch (Exception displayException)
        {
            Log.Error(StartupTag, "Could not display native startup error: " + displayException);
        }
    }
}
