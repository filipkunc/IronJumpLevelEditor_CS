using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GLCanvas;
using IronJumpLevelEditor_CS.PortedClasses;
using System.Diagnostics;

namespace IronJumpLevelEditor_CS
{
    public partial class GameForm : Form
    {
        FPGame game = null;
        HashSet<int> pressedKeys = null;

        public GameForm(GLView sharedContextView)
        {
            InitializeComponent();
            gameView.SharedContextView = sharedContextView;

            game = new FPGame(gameView.Width, gameView.Height);

            FPPlatform platform = new FPPlatform();
            platform.Move(50.0f, 300.0f);
            platform.WidthSegments = 10;
            platform.HeightSegments = 4;

            game.GameObjects.Add(platform);
            pressedKeys = new HashSet<int>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            gameView.Focus();
            timer60FPS.Start();
        }

        private void timer60FPS_Tick(object sender, EventArgs e)
        {
            PointF inputAcceleration = PointF.Empty;

            if (pressedKeys.Contains((int)Keys.Left))
                inputAcceleration.X = -1.0f;
            else if (pressedKeys.Contains((int)Keys.Right))
                inputAcceleration.X = 1.0f;
            if (pressedKeys.Contains((int)Keys.Up))
                inputAcceleration.Y = 1.0f;

            game.InputAcceleration = inputAcceleration;
            game.Update();
            gameView.Invalidate();            
        }

        private void gameView_PaintCanvas(object sender, CanvasEventArgs e)
        {
            Canvas canvas = e.CanvasGL;
            game.Draw(canvas);            
        }

        private void gameView_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyValue);
        }

        private void gameView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            pressedKeys.Add(e.KeyValue);         
        }
    }
}
