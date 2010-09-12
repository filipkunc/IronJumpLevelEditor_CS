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

namespace IronJumpLevelEditor_CS
{
    public partial class DocumentForm : Form
    {
        bool selecting = false;
        Point beginSelection;
        Point endSelection;

        bool texturesLoaded = false;
        Texture ball = null;

        List<Bitmap> images = new List<Bitmap>
        {
            Resources.ball,
            Resources.plos_marble,
            Resources.movable,
            Resources.vytah01,
            Resources.diamond,
            Resources.magnet,
            Resources.speed_symbol,
            Resources.trampoline01,
            Resources.exit
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

        private void levelView_PaintCanvas(object sender, CanvasEventArgs e)
        {
            Canvas canvas = e.CanvasGL;

            if (!texturesLoaded)
            {
                ball = canvas.CreateTexture(Resources.ball);
                texturesLoaded = true;
            }

            canvas.DisableTexturing();
            canvas.EnableBlend();
                
            DrawGrid(canvas);

            canvas.EnableTexturing();
            canvas.SetCurrentColor(Color.White);
            ball.Draw(new PointF(10.0f, 20.0f));

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
            foreach (var image in images)
            {
                rc.Size = image.Size;
                g.DrawImage(image, rc);
                rc.Y += image.Height + 5;
            }
        }

        private void factoryView_Click(object sender, EventArgs e)
        {

        }
    }
}
