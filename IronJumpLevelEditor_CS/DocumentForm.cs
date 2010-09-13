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
    public enum FPDragHandle
    {
        None = 0,

        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight,

        MiddleLeft,
        MiddleTop,
        MiddleRight,
        MiddleBottom,

        Center
    }

    public partial class DocumentForm : Form
    {
        bool texturesLoaded = false;

        GameObjectFactory selectedFactory = null;

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

        List<FPGameObject> gameObjects = new List<FPGameObject>();
        HashSet<int> selectedIndices = new HashSet<int>();

        PointF beginMovePoint;
        PointF endMovePoint;
        FPDragHandle currentHandle = FPDragHandle.None;

        bool drawingSelection = false;
        PointF beginSelection;
        PointF endSelection;

        RectangleF SelectionRect
        {
            get { return beginSelection.RectangleFromPoints(endSelection); }
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

        #region DataSource

        private void AddNewGameObject(FPGameObject gameObject)
        {
            gameObjects.Add(gameObject);
            selectedIndices.Clear();
            selectedIndices.Add(gameObjects.Count - 1);
            if (gameObject.NextPart != null)
                gameObjects.Add(gameObject.NextPart);
	    }

        private void BeginResize(FPDragHandle handle)
        {

        }

        void EndResize(PointF move)
        {

        }

        private void BeginMove()
        {

        }

        void EndMove(PointF move)
        {

        }        

        #endregion

        #region Level View

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

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var gameObject = gameObjects[i];
                canvas.SetCurrentColor(Color.White);
                gameObject.Draw(canvas);
                if (selectedIndices.Contains(i))
                {
                    DrawHandlesOnGameObject(canvas, gameObject);
                }
            }

            var draggedObject = this.DraggedObject;

            if (draggedObject != null)
                DrawHandlesOnGameObject(canvas, draggedObject);

            canvas.DisableTexturing();
            if (drawingSelection)
            {
                canvas.SetCurrentColor(Color.FromArgb(50, Color.White));
                canvas.FillRectangle(SelectionRect);
                canvas.SetCurrentColor(Color.FromArgb(230, Color.White));
                canvas.DrawRectangle(SelectionRect);
            }
        }

        private void levelView_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            x /= 32;
            x *= 32;
            y /= 32;
            y *= 32;
            beginMovePoint.X = x;
            beginMovePoint.Y = y;
            endMovePoint = beginMovePoint;

            if (currentHandle != FPDragHandle.None && currentHandle != FPDragHandle.Center)
            {
                BeginResize(currentHandle);
                return;
            }

            if (selectedFactory != null)
            {
                var draggedObject = selectedFactory.FactoryAction();
                draggedObject.Move(x, y);
                AddNewGameObject(draggedObject);
            }
            else
            {
                Keys flags = Control.ModifierKeys;
                bool startSelection = true;

                if ((flags & Keys.Control) == Keys.Control)
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        var gameObject = gameObjects[i];
                        if (gameObject.Rect.Contains(e.Location))
                        {
                            if (selectedIndices.Contains(i))
                                selectedIndices.Remove(i);
                            else
                                selectedIndices.Add(i);

                            startSelection = false;
                            break;
                        }
                    }
                }
                else if ((flags & Keys.Shift) == Keys.Shift)
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        var gameObject = gameObjects[i];
                        if (gameObject.Rect.Contains(e.Location))
                        {
                            selectedIndices.Add(i);
                            startSelection = false;
                            break;
                        }
                    }
                }
                else if (currentHandle == FPDragHandle.None)
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        var gameObject = gameObjects[i];
                        if (gameObject.Rect.Contains(e.Location))
                        {
                            selectedIndices.Clear();
                            selectedIndices.Add(i);
                            startSelection = false;
                            break;
                        }
                    }
                }
                else
                {
                    startSelection = false;
                    BeginMove();
                }

                if (startSelection)
                {
                    drawingSelection = true;
                    endSelection = beginSelection = e.Location;
                }
            }

            levelView.Invalidate();
        }

        private void levelView_MouseMoved(object sender, MouseEventArgs e)
        {
            currentHandle = FPDragHandle.None;

            const float handleSize = 14.0f;
            RectangleF handleRect = new RectangleF(0.0f, 0.0f, handleSize, handleSize);

            if (selectedIndices.Count == 1)
            {
                var draggedObject = DraggedObject;
                for (FPDragHandle handle = FPDragHandle.TopLeft; handle < FPDragHandle.Center; handle++)
                {
                    if (!RespondsToDragHandle(draggedObject, handle))
                        continue;

                    PointF handlePoint = PointFromHandleAroundRect(handle, draggedObject.Rect);

                    handleRect.X = handlePoint.X - handleRect.Width / 2.0f;
                    handleRect.Y = handlePoint.Y - handleRect.Height / 2.0f;

                    if (handleRect.Contains(e.Location))
                    {
                        currentHandle = handle;
                        beginMovePoint = endMovePoint = e.Location;
                        break;
                    }
                }

                if (currentHandle == FPDragHandle.None)
                {
                    if (draggedObject.Rect.Contains(e.Location))
                    {
                        currentHandle = FPDragHandle.Center;
                        beginMovePoint = endMovePoint = e.Location;
                    }
                }
            }
            else
            {
                foreach (var index in selectedIndices)
                {
                    var gameObject = gameObjects[index];
                    if (gameObject.Rect.Contains(e.Location))
                    {
                        currentHandle = FPDragHandle.Center;
                        beginMovePoint = endMovePoint = e.Location;
                        break;
                    }
                }

            }

            levelView.Invalidate();
        }

        private void levelView_MouseDragged(object sender, MouseEventArgs e)
        {
            if (drawingSelection)
            {
                endSelection = e.Location;
            }
            else if (currentHandle != FPDragHandle.None)
            {
                switch (currentHandle)
                {
                    case FPDragHandle.TopLeft:
                        ResizeDraggedObjectTop(e.Location.Y);
                        ResizeDraggedObjectLeft(e.Location.X);
                        break;
                    case FPDragHandle.TopRight:
                        ResizeDraggedObjectTop(e.Location.Y);
                        ResizeDraggedObjectRight(e.Location.X);
                        break;
                    case FPDragHandle.BottomLeft:
                        ResizeDraggedObjectBottom(e.Location.Y);
                        ResizeDraggedObjectLeft(e.Location.X);
                        break;
                    case FPDragHandle.BottomRight:
                        ResizeDraggedObjectBottom(e.Location.Y);
                        ResizeDraggedObjectRight(e.Location.X);
                        break;
                    case FPDragHandle.MiddleTop:
                        ResizeDraggedObjectTop(e.Location.Y);
                        break;
                    case FPDragHandle.MiddleBottom:
                        ResizeDraggedObjectBottom(e.Location.Y);
                        break;
                    case FPDragHandle.MiddleLeft:
                        ResizeDraggedObjectLeft(e.Location.X);
                        break;
                    case FPDragHandle.MiddleRight:
                        ResizeDraggedObjectRight(e.Location.X);
                        break;
                    case FPDragHandle.Center:
                        if (selectedIndices.Count == 1)
                            MoveDraggedObject(e.Location.X, e.Location.Y);
                        else
                            MoveSelectedObjects(e.Location.X, e.Location.Y);
                        break;
                    default:
                        break;
                }
            }
            else if (selectedFactory != null)
            {
                int widthSegments = (int)((e.Location.X - endMovePoint.X + 16.0f) / 32.0f);
                int heightSegments = (int)((e.Location.Y - endMovePoint.Y + 16.0f) / 32.0f);

                PointF draggedObjectLocation = endMovePoint;
                if (widthSegments < 0)
                    draggedObjectLocation.X += widthSegments * 32.0f;
                if (heightSegments < 0)
                    draggedObjectLocation.Y += heightSegments * 32.0f;

                widthSegments = Math.Max(Math.Abs(widthSegments), 1);
                heightSegments = Math.Max(Math.Abs(heightSegments), 1);

                SetDraggedObjectPosition(draggedObjectLocation.X, draggedObjectLocation.Y);
                var draggedObject = DraggedObject;

                draggedObject.WidthSegments = widthSegments;
                draggedObject.HeightSegments = heightSegments;
            }

            levelView.Invalidate();
        }

        private void levelView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
                levelView_MouseMoved(sender, e);
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
                levelView_MouseDragged(sender, e);
        }

        private void levelView_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawingSelection)
            {
                RectangleF selectionRect = SelectionRect;
                Keys flags = Control.ModifierKeys;

                if ((flags & Keys.Control) == Keys.Control)
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        var gameObject = gameObjects[i];
                        if (selectionRect.IntersectsWith(gameObject.Rect))
                        {
                            if (selectedIndices.Contains(i))
                                selectedIndices.Remove(i);
                            else
                                selectedIndices.Add(i);
                        }
                    }
                }
                else
                {
                    if ((flags & Keys.Shift) != Keys.Shift)
                        selectedIndices.Clear();

                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        var gameObject = gameObjects[i];
                        if (selectionRect.IntersectsWith(gameObject.Rect))
                        {
                            selectedIndices.Add(i);
                        }
                    }
                }

                drawingSelection = false;
            }
            else
            {
                PointF move = endMovePoint;
                move.X -= beginMovePoint.X;
                move.Y -= beginMovePoint.Y;
                if (currentHandle == FPDragHandle.Center)
                    EndMove(move);
                else if (currentHandle != FPDragHandle.None)
                    EndResize(move);
            }

            selectedFactory = null;
            levelView.Invalidate();
            factoryView.Invalidate();
        }
        
        #endregion

        #region Factory View

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
                if (selectedFactory == factory)
                {
                    g.DrawRectangle(new Pen(Color.White, 2.0f), rc);
                }
                rc.Y += factory.Image.Height + 5;
            }
        }

        private void factoryView_Resize(object sender, EventArgs e)
        {
            factoryView.Invalidate();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (GameForm gameForm = new GameForm(levelView, gameObjects))
            {
                gameForm.ShowDialog();
            }
        }

        private void factoryView_MouseClick(object sender, MouseEventArgs e)
        {
            selectedFactory = null;
            Rectangle rc = new Rectangle(8, 5, 32, 32);
            foreach (var factory in factories)
            {
                rc.Size = factory.Image.Size;
                if (rc.Contains(e.Location))
                {
                    selectedFactory = factory;
                    factoryView.Invalidate();
                    return;
                }
                rc.Y += factory.Image.Height + 5;
            }
            factoryView.Invalidate();
        }

        #endregion

        #region Helpers

        private void DrawHandlesOnGameObject(Canvas canvas, FPGameObject gameObject)
        {
            RectangleF rect = gameObject.Rect;

            canvas.DisableTexturing();
            canvas.SetCurrentColor(Color.FromArgb(204, Color.White));
            canvas.DrawRectangle(rect);

            canvas.SetPointSize(6.0f);

            var draggedObject = DraggedObject;

            bool widthHandles = false;
            bool heightHandles = false;

            if (draggedObject != null)
            {
                widthHandles = draggedObject.WidthSegments > 0;
                heightHandles = draggedObject.HeightSegments > 0;
            }

            for (FPDragHandle handle = FPDragHandle.TopLeft; handle < FPDragHandle.Center; handle++)
            {
                if (!widthHandles && IsWidthHandle(handle))
                    continue;

                if (!heightHandles && IsHeightHandle(handle))
                    continue;

                if (handle == currentHandle)
                    canvas.SetCurrentColor(Color.FromArgb(204, Color.Red));
                else
                    canvas.SetCurrentColor(Color.FromArgb(204, Color.Yellow));

                PointF handlePoint = PointFromHandleAroundRect(handle, rect);
                canvas.DrawPoint(handlePoint);
            }
        }

        FPGameObject DraggedObject
        {
            get
            {
                if (selectedIndices.Count == 1)
                    return gameObjects[selectedIndices.First()];
                return null;
            }
        }

        bool RespondsToDragHandle(FPGameObject gameObject, FPDragHandle dragHandle)
        {
            bool respondsToWidth = gameObject.WidthSegments > 0;
            bool respondsToHeight = gameObject.HeightSegments > 0;

            if (!respondsToWidth && IsWidthHandle(dragHandle))
                return false;

            if (!respondsToHeight && IsHeightHandle(dragHandle))
                return false;

            return true;
        }

        void SetDraggedObjectPosition(float x, float y)
        {
            var draggedObject = DraggedObject;
            draggedObject.Move(x - draggedObject.X, y - draggedObject.Y);
        }

        void ResizeDraggedObjectLeft(float x)
        {
            var draggedObject = DraggedObject;
            int widthSegments = (int)((x - endMovePoint.X + 16.0f) / 32.0f);

            if (draggedObject.WidthSegments - widthSegments < 1)
                widthSegments = 0;

            draggedObject.Move(widthSegments * 32.0f, 0.0f);
            endMovePoint.X += widthSegments * 32.0f;
            draggedObject.WidthSegments -= widthSegments;
        }

        void ResizeDraggedObjectRight(float x)
        {
            var draggedObject = DraggedObject;
            int widthSegments = (int)((x - endMovePoint.X + 16.0f) / 32.0f);

            if (draggedObject.WidthSegments + widthSegments < 1)
                widthSegments = 0;

            endMovePoint.X += widthSegments * 32.0f;
            draggedObject.WidthSegments += widthSegments;
        }

        void ResizeDraggedObjectTop(float y)
        {
            var draggedObject = DraggedObject;
            int heightSegments = (int)((y - endMovePoint.Y + 16.0f) / 32.0f);

            if (draggedObject.HeightSegments - heightSegments < 1)
                heightSegments = 0;

            draggedObject.Move(0.0f, heightSegments * 32.0f);
            endMovePoint.Y += heightSegments * 32.0f;
            draggedObject.HeightSegments -= heightSegments;
        }

        void ResizeDraggedObjectBottom(float y)
        {
            var draggedObject = DraggedObject;
            int heightSegments = (int)((y - endMovePoint.Y + 16.0f) / 32.0f);

            if (draggedObject.HeightSegments + heightSegments < 1)
                heightSegments = 0;

            endMovePoint.Y += heightSegments * 32.0f;
            draggedObject.HeightSegments += heightSegments;
        }

        void MoveDraggedObject(float x, float y)
        {
            var draggedObject = DraggedObject;
            int widthSegments = (int)((x - endMovePoint.X + 16.0f) / 32.0f);
            int heightSegments = (int)((y - endMovePoint.Y + 16.0f) / 32.0f);

            draggedObject.Move(widthSegments * 32.0f, heightSegments * 32.0f);
            endMovePoint.X += widthSegments * 32.0f;
            endMovePoint.Y += heightSegments * 32.0f;
        }

        void MoveSelectedObjects(float x, float y)
        {
            int widthSegments = (int)((x - endMovePoint.X + 16.0f) / 32.0f);
            int heightSegments = (int)((y - endMovePoint.Y + 16.0f) / 32.0f);

            foreach (var index in selectedIndices)
            {
                var gameObject = gameObjects[index];
                gameObject.Move(widthSegments * 32.0f, heightSegments * 32.0f);
            }

            endMovePoint.X += widthSegments * 32.0f;
            endMovePoint.Y += heightSegments * 32.0f;
        }

        bool IsWidthHandle(FPDragHandle handle)
        {
            switch (handle)
            {
                case FPDragHandle.TopLeft:
                case FPDragHandle.BottomLeft:
                case FPDragHandle.TopRight:
                case FPDragHandle.BottomRight:
                case FPDragHandle.MiddleLeft:
                case FPDragHandle.MiddleRight:
                    return true;
                case FPDragHandle.MiddleTop:
                case FPDragHandle.MiddleBottom:
                case FPDragHandle.Center:
                case FPDragHandle.None:
                default:
                    return false;
            }
        }

        bool IsHeightHandle(FPDragHandle handle)
        {
            switch (handle)
            {
                case FPDragHandle.TopLeft:
                case FPDragHandle.BottomLeft:
                case FPDragHandle.TopRight:
                case FPDragHandle.BottomRight:
                case FPDragHandle.MiddleTop:
                case FPDragHandle.MiddleBottom:
                    return true;
                case FPDragHandle.MiddleLeft:
                case FPDragHandle.MiddleRight:
                case FPDragHandle.Center:
                case FPDragHandle.None:
                default:
                    return false;
            }
        }

        PointF PointFromHandleAroundRect(FPDragHandle handle, RectangleF rect)
        {
            switch (handle)
            {
                case FPDragHandle.TopLeft:
                    return new PointF(rect.X, rect.Y);
                case FPDragHandle.BottomLeft:
                    return new PointF(rect.X, rect.Y + rect.Height);
                case FPDragHandle.TopRight:
                    return new PointF(rect.X + rect.Width, rect.Y);
                case FPDragHandle.BottomRight:
                    return new PointF(rect.X + rect.Width, rect.Y + rect.Height);

                case FPDragHandle.MiddleLeft:
                    return new PointF(rect.X, rect.Y + rect.Height / 2.0f);
                case FPDragHandle.MiddleTop:
                    return new PointF(rect.X + rect.Width / 2.0f, rect.Y);
                case FPDragHandle.MiddleRight:
                    return new PointF(rect.X + rect.Width, rect.Y + rect.Height / 2.0f);
                case FPDragHandle.MiddleBottom:
                    return new PointF(rect.X + rect.Width / 2.0f, rect.Y + rect.Height);
                case FPDragHandle.Center:
                    return new PointF(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);

                default:
                    return PointF.Empty;
            }
        }

        #endregion
    }
}
