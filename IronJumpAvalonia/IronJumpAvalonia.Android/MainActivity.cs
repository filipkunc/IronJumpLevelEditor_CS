using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Avalonia;
using Avalonia.Android;

namespace IronJumpAvalonia.Android
{
	[Activity(
		Label = "IronJumpAvalonia.Android",
		Theme = "@style/MyTheme.NoActionBar",
		Icon = "@drawable/icon",
		MainLauncher = true,
		ScreenOrientation = ScreenOrientation.Landscape,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
	public class MainActivity : AvaloniaMainActivity<App>
	{
		protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
		{
			return base.CustomizeAppBuilder(builder)
				.WithInterFont();
		}

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
		}
	}
}
