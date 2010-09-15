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
using System.Xml.Linq;
using System.Reflection;

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
        UndoManager undoManager = new UndoManager();

        bool texturesLoaded = false;

        GameObjectFactory selectedFactory = null;

        List<GameObjectFactory> factories = new List<GameObjectFactory>
        {
            new GameObjectFactory (Resources.ball, () => new FPPlayer()),
            new GameObjectFactory (Resources.plos_marble, () => new FPPlatform()),
            new GameObjectFactory (Resources.movable, () => new FPMovablePlatform()),
            new GameObjectFactory (Resources.vytah01, () => new FPElevator()),
            new GameObjectFactory (Resources.diamond, () => new FPDiamond()),
            new GameObjectFactory (Resources.magnet, () => new FPMagnet()),
            new GameObjectFactory (Resources.speed_symbol, () => new FPSpeedPowerUp()),
            new GameObjectFactory (Resources.trampoline01, () => new FPTrampoline()),
            new GameObjectFactory (Resources.exit, () => new FPExit()),            
        };

        List<FPGameObject> gameObjects = new List<FPGameObject>();
        SortedSet<int> selectedIndices = new SortedSet<int>();

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

        GameObjectFactory SelectedFactory
        {
            get { return selectedFactory; }
            set
            {
                if (previousObjects != null)
                    AfterAction("Add New Object");
                selectedFactory = value;
                factoryView.Invalidate();
            }
        }

        public DocumentForm()
        {
            InitializeComponent();
            factoryView.BackColor = Color.FromArgb(55, 60, 89);
            levelView_SizeChanged(this, EventArgs.Empty);
        }

        private void InitAllTextures(FPCanvas canvas)
        {
            FPGame.InitTexture(canvas);
            FPPlayer.InitTextures(canvas);
            FPPlatform.InitTextures(canvas);
            FPDiamond.InitTextures(canvas);
            FPElevator.InitTextures(canvas);
            FPMovablePlatform.InitTextures(canvas);
            FPMagnet.InitTextures(canvas);
            FPSpeedPowerUp.InitTextures(canvas);
            FPTrampoline.InitTextures(canvas);
            FPExit.InitTextures(canvas);
        }

        void DrawGrid(FPCanvas canvas)
        {
            Rectangle rect = levelView.ClientRectangle;

            rect.Offset(-(int)levelView.ViewOffset.X, -(int)levelView.ViewOffset.Y);

            rect.X /= 32;
            rect.Y /= 32;

            rect.X *= 32;
            rect.Y *= 32;

            canvas.SetCurrentColor(Color.FromArgb((int)(0.2 * 255), Color.White));

            for (int y = rect.Top; y < rect.Bottom + 32; y += 32)
                canvas.DrawLine(new Point(rect.Left, y), new Point(rect.Right + 32, y));

            for (int x = rect.Left; x < rect.Right + 32; x += 32)
                canvas.DrawLine(new Point(x, rect.Top), new Point(x, rect.Bottom + 32));
        }

        #region Undo & Redo

        private void SwapOldWithNew(List<FPGameObject> oldObjects, SortedSet<int> oldIndices,
            List<FPGameObject> newObjects, SortedSet<int> newIndices, string name)
        {
            gameObjects.Clear();
            gameObjects.AddRange(oldObjects);
            selectedIndices.Clear();
            foreach (var index in oldIndices)
                selectedIndices.Add(index);

            undoManager.PrepareUndo(name, Invocation.Create(this,
                form => form.SwapOldWithNew(newObjects, newIndices, oldObjects, oldIndices, name)));

            levelView.Invalidate();
        }

        private void DuplicateCurrentObjectsAndIndices(List<FPGameObject> currentObjects, SortedSet<int> currentIndices)
        {
            currentObjects.Clear();

            foreach (var gameObject in gameObjects)
            {
                var duplicate = gameObject.Duplicate(0.0f, 0.0f);
                if (duplicate != null)
                {
                    currentObjects.Add(duplicate);
                }
                else // elevator end
                {
                    var parentInGameObjects = gameObject.NextPart;
                    var parentInOldObjects = currentObjects[gameObjects.IndexOf(parentInGameObjects)];
                    var nextPart = parentInOldObjects.NextPart;
                    currentObjects.Add(nextPart);
                }
            }

            currentIndices.Clear();
            foreach (var index in selectedIndices)
                currentIndices.Add(index);
        }

        private List<FPGameObject> previousObjects = null;
        private SortedSet<int> previousIndices = null;

        private void BeforeAction()
        {
            if (previousObjects != null || previousIndices != null)
                throw new ApplicationException("BeforeAction called twice");

            previousObjects = new List<FPGameObject>();
            previousIndices = new SortedSet<int>();

            DuplicateCurrentObjectsAndIndices(previousObjects, previousIndices);
        }

        private void FullAction(string name, Action<DocumentForm> action)
        {
            BeforeAction();
            action(this);
            AfterAction(name);
        }

        private void AfterAction(string name)
        {
            undoManager.PrepareUndo(name, Invocation.Create(previousObjects, previousIndices,
                (oldObjects, oldIndices) =>
                {
                    List<FPGameObject> currentObjects = new List<FPGameObject>();
                    SortedSet<int> currentIndices = new SortedSet<int>();
                    DuplicateCurrentObjectsAndIndices(currentObjects, currentIndices);

                    SwapOldWithNew(oldObjects, oldIndices, currentObjects, currentIndices, name);                    
                }));

            previousObjects = null;
            previousIndices = null;
        }

        #endregion

        #region DataSource

        private void AddNewGameObject(FPGameObject gameObject)
        {
            BeforeAction();
            gameObjects.Add(gameObject);
            selectedIndices.Clear();
            selectedIndices.Add(gameObjects.Count - 1);
            if (gameObject.NextPart != null)
                gameObjects.Add(gameObject.NextPart);
        }

        private void BeginResize(FPDragHandle handle)
        {
            BeforeAction();
        }

        void EndResize(PointF move)
        {
            AfterAction("Resize Selected");
        }

        private void BeginMove()
        {
            BeforeAction();
        }

        void EndMove(PointF move)
        {
            AfterAction("Move Selected");
        }

        #endregion

        #region Level View

        private void levelView_PaintCanvas(object sender, CanvasEventArgs e)
        {
            FPCanvas canvas = e.Canvas;

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

        private PointF LevelViewLocation(MouseEventArgs e)
        {
            PointF location = new PointF();
            location.X = (float)e.Location.X - levelView.ViewOffset.X;
            location.Y = (float)e.Location.Y - levelView.ViewOffset.Y;
            return location;
        }

        private void levelView_MouseDown(object sender, MouseEventArgs e)
        {
            PointF location = LevelViewLocation(e);
            int x = (int)location.X;
            int y = (int)location.Y;
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

            if (SelectedFactory != null)
            {
                var draggedObject = SelectedFactory.FactoryAction();
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
                        if (gameObject.Rect.Contains(location))
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
                        if (gameObject.Rect.Contains(location))
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
                        if (gameObject.Rect.Contains(location))
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
                    endSelection = beginSelection = location;
                }
            }

            levelView.Invalidate();
        }

        private void levelView_MouseMoved(object sender, MouseEventArgs e)
        {
            PointF location = LevelViewLocation(e);
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

                    if (handleRect.Contains(location))
                    {
                        currentHandle = handle;
                        beginMovePoint = endMovePoint = location;
                        break;
                    }
                }

                if (currentHandle == FPDragHandle.None)
                {
                    if (draggedObject.Rect.Contains(location))
                    {
                        currentHandle = FPDragHandle.Center;
                        beginMovePoint = endMovePoint = location;
                    }
                }
            }
            else
            {
                foreach (var index in selectedIndices)
                {
                    var gameObject = gameObjects[index];
                    if (gameObject.Rect.Contains(location))
                    {
                        currentHandle = FPDragHandle.Center;
                        beginMovePoint = endMovePoint = location;
                        break;
                    }
                }

            }

            levelView.Invalidate();
        }

        private void levelView_MouseDragged(object sender, MouseEventArgs e)
        {
            PointF location = LevelViewLocation(e);
            if (drawingSelection)
            {
                endSelection = location;
            }
            else if (currentHandle != FPDragHandle.None)
            {
                switch (currentHandle)
                {
                    case FPDragHandle.TopLeft:
                        ResizeDraggedObjectTop(location.Y);
                        ResizeDraggedObjectLeft(location.X);
                        break;
                    case FPDragHandle.TopRight:
                        ResizeDraggedObjectTop(location.Y);
                        ResizeDraggedObjectRight(location.X);
                        break;
                    case FPDragHandle.BottomLeft:
                        ResizeDraggedObjectBottom(location.Y);
                        ResizeDraggedObjectLeft(location.X);
                        break;
                    case FPDragHandle.BottomRight:
                        ResizeDraggedObjectBottom(location.Y);
                        ResizeDraggedObjectRight(location.X);
                        break;
                    case FPDragHandle.MiddleTop:
                        ResizeDraggedObjectTop(location.Y);
                        break;
                    case FPDragHandle.MiddleBottom:
                        ResizeDraggedObjectBottom(location.Y);
                        break;
                    case FPDragHandle.MiddleLeft:
                        ResizeDraggedObjectLeft(location.X);
                        break;
                    case FPDragHandle.MiddleRight:
                        ResizeDraggedObjectRight(location.X);
                        break;
                    case FPDragHandle.Center:
                        if (selectedIndices.Count == 1)
                            MoveDraggedObject(location.X, location.Y);
                        else
                            MoveSelectedObjects(location.X, location.Y);
                        break;
                    default:
                        break;
                }
            }
            else if (SelectedFactory != null)
            {
                int widthSegments = (int)((location.X - endMovePoint.X + 16.0f) / 32.0f);
                int heightSegments = (int)((location.Y - endMovePoint.Y + 16.0f) / 32.0f);

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

            SelectedFactory = null;
            levelView.Invalidate();            
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
                if (SelectedFactory == factory)
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

        private void factoryView_MouseClick(object sender, MouseEventArgs e)
        {
            SelectedFactory = null;
            Rectangle rc = new Rectangle(8, 5, 32, 32);
            foreach (var factory in factories)
            {
                rc.Size = factory.Image.Size;
                if (rc.Contains(e.Location))
                {
                    SelectedFactory = factory;
                    return;
                }
                rc.Y += factory.Image.Height + 5;
            }            
        }

        #endregion

        #region Helpers

        private void DrawHandlesOnGameObject(FPCanvas canvas, FPGameObject gameObject)
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

        #region Main Menu

        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previousObjects = null;
            previousIndices = null;
            undoManager = new UndoManager();
            gameObjects.Clear();
            SelectedFactory = null;
            levelView.Invalidate();            
        }

        private void openLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                previousObjects = null;
                previousIndices = null;
                undoManager = new UndoManager();
                gameObjects.Clear();
                SelectedFactory = null;

                XElement root = XElement.Load(openFileDialog.FileName);
                foreach (var element in root.Elements())
                {
                    var type = Type.GetType("IronJumpLevelEditor_CS.PortedClasses." + element.Name.ToString());
                    var gameObject = (FPGameObject)Activator.CreateInstance(type);
                    gameObject.InitFromElement(element);
                    gameObjects.Add(gameObject);
                    if (gameObject.NextPart != null)
                        gameObjects.Add(gameObject.NextPart);
                }

                levelView.Invalidate();
            }
        }

        private void saveLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                XElement root = new XElement("IronJumpLevel");
                foreach (var gameObject in gameObjects)
                {
                    if (gameObject is FPElevatorEnd)
                        continue;

                    XElement gameObjectElement = new XElement(gameObject.GetType().Name);
                    gameObject.WriteToElement(gameObjectElement);
                    root.Add(gameObjectElement);
                }
                root.Save(saveFileDialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoManager.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoManager.Redo();
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullAction("Duplicate Selected",
                form =>
                {
                    List<FPGameObject> duplicates = new List<FPGameObject>();
                    List<FPGameObject> nextParts = new List<FPGameObject>();

                    foreach (var index in selectedIndices)
                    {
                        var gameObject = gameObjects[index];
                        var duplicate = gameObject.Duplicate(32.0f, 32.0f);
                        if (duplicate != null)
                        {
                            duplicates.Add(duplicate);
                            if (duplicate.NextPart != null)
                            {
                                nextParts.Add(duplicate.NextPart);
                            }
                        }
                    }

                    selectedIndices.Clear();
                    gameObjects.AddRange(duplicates);
                    for (int i = gameObjects.Count - duplicates.Count; i < gameObjects.Count; i++)
                        selectedIndices.Add(i);

                    gameObjects.AddRange(nextParts);
                    for (int i = gameObjects.Count - nextParts.Count; i < gameObjects.Count; i++)
                        selectedIndices.Add(i);

                    levelView.Invalidate();
                });
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullAction("Delete Selected",
                form =>
                {
                    List<FPGameObject> nextParts = new List<FPGameObject>();
                    foreach (var index in selectedIndices)
                    {
                        var gameObject = gameObjects[index];
                        if (gameObject.NextPart != null)
                        {
                            nextParts.Add(gameObject.NextPart);
                        }
                        if (nextParts.Contains(gameObject))
                            nextParts.Remove(gameObject);
                    }

                    int diff = 0;
                    foreach (var index in selectedIndices)
                    {
                        gameObjects.RemoveAt(index - diff);
                        diff++;
                    }

                    gameObjects.RemoveAll(x => nextParts.Contains(x));
                    selectedIndices.Clear();
                    levelView.Invalidate();
                });
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedIndices.Clear();
            for (int i = 0; i < gameObjects.Count; i++)
                selectedIndices.Add(i);
            levelView.Invalidate();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (GameForm gameForm = new GameForm(levelView, gameObjects))
            {
                gameForm.ShowDialog();
            }
        }

        #endregion

        #region Scrolling

        private void levelView_SizeChanged(object sender, EventArgs e)
        {
            Size size = levelView.ClientSize;
            hScrollBarLevelView.Maximum = Math.Max(8000 - size.Width, 0);
            vScrollBarLevelView.Maximum = Math.Max(8000 - size.Height, 0);
        }

        private void hScrollBarLevelView_ValueChanged(object sender, EventArgs e)
        {
            levelView.ViewOffset = new PointF(-hScrollBarLevelView.Value, -vScrollBarLevelView.Value);
            levelView.Invalidate();
        }

        private void vScrollBarLevelView_ValueChanged(object sender, EventArgs e)
        {
            levelView.ViewOffset = new PointF(-hScrollBarLevelView.Value, -vScrollBarLevelView.Value);
            levelView.Invalidate();
        }

        #endregion
    }
}
