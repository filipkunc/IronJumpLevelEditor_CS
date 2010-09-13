using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;
using IronJumpLevelEditor_CS.PortedClasses;

namespace IronJumpLevelEditor_CS
{
    public partial class DocumentForm : Form
    {
        bool selecting = false;
        Point beginSelection;
        Point endSelection;

        bool texturesLoaded = false;
        
        List<GameObjectFactory> factories = new List<GameObjectFactory>
        {
            new GameObjectFactory (Resources.ball, () => new FPPlayer()),
            new GameObjectFactory (Resources.plos_marble, () => new FPPlatform()),
            new GameObjectFactory (Resources.movable, () => null),
            new GameObjectFactory (Resources.vytah01, () => null),
            new GameObjectFactory (Resources.diamond, () => null),
            new GameObjectFactory (Resources.magnet, () => null),
            new GameObjectFactory (Resources.speed_symbol, () => null),
            new GameObjectFactory (Resources.trampoline01, () => null),
            new GameObjectFactory (Resources.exit, () => null),            
        };

        Rectangle SelectionRect
        {
            get
            {
                int left = Math.Min(beginSelection.X, endSelection.X);
                int top = Math.Min(beginSelection.Y, endSelection.Y);
                int right = Math.Max(beginSelection.X, endSelection.X);
                int bottom = Math.Max(beginSelection.Y, endSelection.Y);

                return Rectangle.FromLTRB(left, top, right, bottom);
            }
        }

        public DocumentForm()
        {
            InitializeComponent();
            factoryView.BackColor = Color.FromArgb(55, 60, 89);           
        }

        private void InitAllTextures(Canvas canvas)
        {
            FPGame.InitTexture(canvas);
            FPPlayer.InitTextures(canvas);
            FPPlatform.InitTextures(canvas);            
        }

        private void levelView_PaintCanvas(object sender, CanvasEventArgs e)
        {
            Canvas canvas = e.CanvasGL;

            if (!texturesLoaded)
            {
                InitAllTextures(canvas);
                texturesLoaded = true;
            }

            canvas.DisableTexturing();
            canvas.EnableBlend();
                
            DrawGrid(canvas);

            canvas.EnableTexturing();
            canvas.SetCurrentColor(Color.White);

            canvas.DisableTexturing();
            DrawSelection(canvas);
        }

        void DrawGrid(Canvas canvas)
        {
            Rectangle rect = levelView.ClientRectangle;
            rect.X = -rect.Width;
            rect.Y = -rect.Height;

            rect.X /= 32;
            rect.Y /= 32;

            rect.X *= 32;
            rect.Y *= 32;

            canvas.SetCurrentColor(Color.FromArgb((int)(0.2 * 255), Color.White));

            for (int y = rect.Y; y < rect.Height; y += 32)
                canvas.DrawLine(new Point(rect.X, y), new Point(rect.Width, y));

            for (int x = rect.X; x < rect.Width; x += 32)
                canvas.DrawLine(new Point(x, rect.Y), new Point(x, rect.Height));
        }

        void DrawSelection(Canvas canvas)
        {
            if (selecting)
            {
                canvas.SetCurrentColor(Color.FromArgb((int)(0.2 * 255), Color.White));
                canvas.FillRectangle(SelectionRect);
                canvas.SetCurrentColor(Color.FromArgb((int)(0.9 * 255), Color.White));
                canvas.DrawRectangle(SelectionRect);
            }
        }

        private void levelView_MouseMove(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                endSelection = e.Location;
                levelView.Invalidate();
            }
        }

        private void levelView_MouseDown(object sender, MouseEventArgs e)
        {
            beginSelection = endSelection = e.Location;
            selecting = true;
        }

        private void levelView_MouseUp(object sender, MouseEventArgs e)
        {
            endSelection = e.Location;
            selecting = false;
            levelView.Invalidate();
        }

        private void factoryView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle borderRect = factoryView.ClientRectangle;
            
            borderRect.Width--;
            borderRect.Height--;
            g.DrawRectangle(new Pen(Color.FromArgb(100, Color.White)), borderRect);

            Rectangle rc = new Rectangle(8, 5, 32, 32);
            foreach (var factory in factories)
            {
                rc.Size = factory.Image.Size;
                g.DrawImage(factory.Image, rc);
                rc.Y += factory.Image.Height + 5;
            }
        }

        private void factoryView_Click(object sender, EventArgs e)
        {

        }

        private void factoryView_Resize(object sender, EventArgs e)
        {
            factoryView.Invalidate();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (GameForm gameForm = new GameForm(levelView))
            {
                gameForm.ShowDialog();
            }
        }
    }
}
