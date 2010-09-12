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
            this.factoryView = new System.Windows.Forms.PictureBox();
            this.levelView = new GLCanvas.GLView();
            ((System.ComponentModel.ISupportInitialize)(this.factoryView)).BeginInit();
            this.SuspendLayout();
            // 
            // factoryView
            // 
            this.factoryView.BackColor = System.Drawing.Color.LightSlateGray;
            this.factoryView.Dock = System.Windows.Forms.DockStyle.Left;
            this.factoryView.Location = new System.Drawing.Point(0, 0);
            this.factoryView.MaximumSize = new System.Drawing.Size(80, 0);
            this.factoryView.MinimumSize = new System.Drawing.Size(80, 0);
            this.factoryView.Name = "factoryView";
            this.factoryView.Size = new System.Drawing.Size(80, 487);
            this.factoryView.TabIndex = 0;
            this.factoryView.TabStop = false;
            this.factoryView.Click += new System.EventHandler(this.factoryView_Click);
            this.factoryView.Paint += new System.Windows.Forms.PaintEventHandler(this.factoryView_Paint);
            // 
            // levelView
            // 
            this.levelView.BackColor = System.Drawing.Color.Silver;
            this.levelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.levelView.Location = new System.Drawing.Point(80, 0);
            this.levelView.Name = "levelView";
            this.levelView.Size = new System.Drawing.Size(647, 487);
            this.levelView.TabIndex = 0;
            this.levelView.TabStop = false;
            this.levelView.PaintCanvas += new System.EventHandler<GLCanvas.CanvasEventArgs>(this.levelView_PaintCanvas);
            this.levelView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.levelView_MouseDown);
            this.levelView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.levelView_MouseMove);
            this.levelView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.levelView_MouseUp);
            // 
            // DocumentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 487);
            this.Controls.Add(this.levelView);
            this.Controls.Add(this.factoryView);
            this.Name = "DocumentForm";
            this.Text = "IronJump Level Editor";
            ((System.ComponentModel.ISupportInitialize)(this.factoryView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GLCanvas.GLView levelView;
        private System.Windows.Forms.PictureBox factoryView;
    }
}

