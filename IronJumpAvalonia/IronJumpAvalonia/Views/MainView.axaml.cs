using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.IO;

namespace IronJumpAvalonia.Views
{
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();
			levelEditor.FactoryView = factoryView;
		}

		private async void OpenLevel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			// Get top level from the current control. Alternatively, you can use Window reference instead.
			var topLevel = TopLevel.GetTopLevel(this);

			// Start async operation to open the dialog.
			var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				Title = "Open Level",
				AllowMultiple = false
			});

			if (files.Count >= 1)
			{
				// Open reading stream from the first file.
				await using var stream = await files[0].OpenReadAsync();
				using var streamReader = new StreamReader(stream);
				//// Reads all the content of file as a text.
				var fileContent = await streamReader.ReadToEndAsync();
				levelEditor.LoadLevel(fileContent);
			}
		}

		private void GamePlay_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			levelEditor.Play();
		}
	}
}