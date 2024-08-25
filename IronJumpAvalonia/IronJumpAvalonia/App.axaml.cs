using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using IronJumpAvalonia.Controls;
using IronJumpAvalonia.Game;
using IronJumpAvalonia.ViewModels;
using IronJumpAvalonia.Views;
using SkiaSharp;
using System.IO;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace IronJumpAvalonia
{
	public partial class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public async override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				// Line below is needed to remove Avalonia data validation.
				// Without this line you will get duplicate validations from both Avalonia and CT
				BindingPlugins.DataValidators.RemoveAt(0);
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainViewModel()
				};
			}
			else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
			{
				FPGame.InitTexture();
				FPPlayer.InitTextures();
				FPPlatform.InitTextures();
				FPDiamond.InitTextures();
				FPElevator.InitTextures();
				FPMovablePlatform.InitTextures();
				FPMagnet.InitTextures();
				FPSpeedPowerUp.InitTextures();
				FPTrampoline.InitTextures();
				FPExit.InitTextures();

				const string fileName = "windows.xlevel";

				using var stream = AssetLoader.Open(new Uri($"avares://IronJumpAvalonia/Assets/{fileName}"));
				using var streamReader = new StreamReader(stream);
				//// Reads all the content of file as a text.
				var fileContent = await streamReader.ReadToEndAsync();

				List<FPGameObject> gameObjects = new List<FPGameObject>();

				XElement root = XElement.Parse(fileContent);
				foreach (var element in root.Elements())
				{
					var type = Type.GetType("IronJumpAvalonia.Game." + element.Name.ToString());
					var gameObject = (FPGameObject)Activator.CreateInstance(type);
					gameObject.InitFromElement(element);
					gameObjects.Add(gameObject);
					if (gameObject.NextPart != null)
						gameObjects.Add(gameObject.NextPart);
				}

				singleViewPlatform.MainView = new GamePlayer
				{
					Game = new FPGame(480, 320, gameObjects)
				};

			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}