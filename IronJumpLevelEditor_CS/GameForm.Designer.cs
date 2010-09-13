namespace IronJumpLevelEditor_CS
{
    partial class GameForm
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
            this.components = new System.ComponentModel.Container();
            this.gameView = new GLCanvas.GLView();
            this.timer60FPS = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // gameView
            // 
            this.gameView.BackColor = System.Drawing.Color.LightSlateGray;
            this.gameView.Location = new System.Drawing.Point(0, 0);
            this.gameView.Margin = new System.Windows.Forms.Padding(0);
            this.gameView.MaximumSize = new System.Drawing.Size(480, 320);
            this.gameView.MinimumSize = new System.Drawing.Size(480, 320);
            this.gameView.Name = "gameView";
            this.gameView.SharedContextView = null;
            this.gameView.Size = new System.Drawing.Size(480, 320);
            this.gameView.TabIndex = 0;
            this.gameView.TabStop = false;
            this.gameView.PaintCanvas += new System.EventHandler<GLCanvas.CanvasEventArgs>(this.gameView_PaintCanvas);
            this.gameView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gameView_KeyUp);
            this.gameView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gameView_PreviewKeyDown);
            // 
            // timer60FPS
            // 
            this.timer60FPS.Interval = 15;
            this.timer60FPS.Tick += new System.EventHandler(this.timer60FPS_Tick);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 320);
            this.Controls.Add(this.gameView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game Simulation";
            this.ResumeLayout(false);

        }

        #endregion

        private GLCanvas.GLView gameView;
        private System.Windows.Forms.Timer timer60FPS;
    }
}