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

        public GameForm(GLView sharedContextView, IEnumerable<FPGameObject> gameObjects)
        {
            InitializeComponent();
            gameView.SharedContextView = sharedContextView;

            game = new FPGame(gameView.Width, gameView.Height);
            pressedKeys = new HashSet<int>();

            PointF playerPosition = PointF.Empty;

            foreach (var gameObject in gameObjects)
            {
                if (gameObject is FPPlayer)
                {
                    playerPosition.X = gameObject.X;
                    playerPosition.Y = gameObject.Y;
                }
                else
                {
                    game.GameObjects.Add(gameObject.Duplicate(0.0f, 0.0f));
                }
            }

            game.MoveWorld(game.Player.X - playerPosition.X, game.Player.Y - playerPosition.Y);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
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
