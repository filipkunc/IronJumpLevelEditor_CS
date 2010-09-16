namespace IronJumpLevelEditor_CS
{
    partial class DocumentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentForm));
            this.factoryView = new System.Windows.Forms.PictureBox();
            this.levelView = new GLCanvas.GLView();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLevelAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutIronJumpLevelEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hScrollBarLevelView = new System.Windows.Forms.HScrollBar();
            this.vScrollBarLevelView = new System.Windows.Forms.VScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.factoryView)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // factoryView
            // 
            this.factoryView.BackColor = System.Drawing.Color.LightSlateGray;
            this.factoryView.Dock = System.Windows.Forms.DockStyle.Left;
            this.factoryView.Location = new System.Drawing.Point(0, 24);
            this.factoryView.MaximumSize = new System.Drawing.Size(80, 0);
            this.factoryView.MinimumSize = new System.Drawing.Size(80, 0);
            this.factoryView.Name = "factoryView";
            this.factoryView.Size = new System.Drawing.Size(80, 463);
            this.factoryView.TabIndex = 0;
            this.factoryView.TabStop = false;
            this.factoryView.Paint += new System.Windows.Forms.PaintEventHandler(this.factoryView_Paint);
            this.factoryView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.factoryView_MouseClick);
            this.factoryView.Resize += new System.EventHandler(this.factoryView_Resize);
            // 
            // levelView
            // 
            this.levelView.BackColor = System.Drawing.Color.Silver;
            this.levelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.levelView.Location = new System.Drawing.Point(80, 24);
            this.levelView.Name = "levelView";
            this.levelView.SharedContextView = null;
            this.levelView.Size = new System.Drawing.Size(630, 446);
            this.levelView.TabIndex = 0;
            this.levelView.TabStop = false;
            this.levelView.ViewOffset = ((System.Drawing.PointF)(resources.GetObject("levelView.ViewOffset")));
            this.levelView.PaintCanvas += new System.EventHandler<GLCanvas.CanvasEventArgs>(this.levelView_PaintCanvas);
            this.levelView.SizeChanged += new System.EventHandler(this.levelView_SizeChanged);
            this.levelView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.levelView_MouseDown);
            this.levelView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.levelView_MouseMove);
            this.levelView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.levelView_MouseUp);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.gameToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(727, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newLevelToolStripMenuItem,
            this.openLevelToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveLevelToolStripMenuItem,
            this.saveLevelAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newLevelToolStripMenuItem
            // 
            this.newLevelToolStripMenuItem.Name = "newLevelToolStripMenuItem";
            this.newLevelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newLevelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.newLevelToolStripMenuItem.Text = "New Level";
            this.newLevelToolStripMenuItem.Click += new System.EventHandler(this.newLevelToolStripMenuItem_Click);
            // 
            // openLevelToolStripMenuItem
            // 
            this.openLevelToolStripMenuItem.Name = "openLevelToolStripMenuItem";
            this.openLevelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openLevelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.openLevelToolStripMenuItem.Text = "Open Level...";
            this.openLevelToolStripMenuItem.Click += new System.EventHandler(this.openLevelToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(188, 6);
            // 
            // saveLevelToolStripMenuItem
            // 
            this.saveLevelToolStripMenuItem.Name = "saveLevelToolStripMenuItem";
            this.saveLevelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveLevelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveLevelToolStripMenuItem.Text = "Save Level";
            this.saveLevelToolStripMenuItem.Click += new System.EventHandler(this.saveLevelToolStripMenuItem_Click);
            // 
            // saveLevelAsToolStripMenuItem
            // 
            this.saveLevelAsToolStripMenuItem.Name = "saveLevelAsToolStripMenuItem";
            this.saveLevelAsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveLevelAsToolStripMenuItem.Text = "Save Level As...";
            this.saveLevelAsToolStripMenuItem.Click += new System.EventHandler(this.saveLevelAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator1,
            this.duplicateToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(165, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutIronJumpLevelEditorToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutIronJumpLevelEditorToolStripMenuItem
            // 
            this.aboutIronJumpLevelEditorToolStripMenuItem.Name = "aboutIronJumpLevelEditorToolStripMenuItem";
            this.aboutIronJumpLevelEditorToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.aboutIronJumpLevelEditorToolStripMenuItem.Text = "About";
            this.aboutIronJumpLevelEditorToolStripMenuItem.Click += new System.EventHandler(this.aboutIronJumpLevelEditorToolStripMenuItem_Click);
            // 
            // hScrollBarLevelView
            // 
            this.hScrollBarLevelView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBarLevelView.Location = new System.Drawing.Point(80, 470);
            this.hScrollBarLevelView.Name = "hScrollBarLevelView";
            this.hScrollBarLevelView.Size = new System.Drawing.Size(630, 17);
            this.hScrollBarLevelView.TabIndex = 2;
            this.hScrollBarLevelView.ValueChanged += new System.EventHandler(this.hScrollBarLevelView_ValueChanged);
            // 
            // vScrollBarLevelView
            // 
            this.vScrollBarLevelView.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBarLevelView.Location = new System.Drawing.Point(0, 0);
            this.vScrollBarLevelView.Name = "vScrollBarLevelView";
            this.vScrollBarLevelView.Size = new System.Drawing.Size(17, 446);
            this.vScrollBarLevelView.TabIndex = 3;
            this.vScrollBarLevelView.ValueChanged += new System.EventHandler(this.vScrollBarLevelView_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 446);
            this.panel1.MaximumSize = new System.Drawing.Size(17, 17);
            this.panel1.MinimumSize = new System.Drawing.Size(17, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(17, 17);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.vScrollBarLevelView);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(710, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(17, 463);
            this.panel2.TabIndex = 5;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xlevel";
            this.openFileDialog.Filter = "XML levels|*.xlevel";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xlevel";
            this.saveFileDialog.Filter = "XML levels|*.xlevel";
            // 
            // DocumentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 487);
            this.Controls.Add(this.levelView);
            this.Controls.Add(this.hScrollBarLevelView);
            this.Controls.Add(this.factoryView);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "DocumentForm";
            this.Text = "IronJump Level Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.factoryView)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GLCanvas.GLView levelView;
        private System.Windows.Forms.PictureBox factoryView;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.HScrollBar hScrollBarLevelView;
        private System.Windows.Forms.VScrollBar vScrollBarLevelView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem saveLevelAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutIronJumpLevelEditorToolStripMenuItem;
    }
}

