using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IronJumpAvalonia.Controls;
using IronJumpAvalonia.Game;

namespace IronJumpAvalonia;

public partial class GameWindow : Window
{
    public GameWindow()
    {
        InitializeComponent();
    }

    public FPGame Game
    {
        get { return gamePlayer.Game; }
        set { gamePlayer.Game = value; }
    }
}
