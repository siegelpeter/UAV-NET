namespace GroundControlUI
{
    partial class EFISViewer
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.glefis1 = new OpenGLUI.GLAI();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // glefis1
            // 
            this.glefis1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glefis1.Location = new System.Drawing.Point(0, 0);
            this.glefis1.Name = "glefis1";
            this.glefis1.Size = new System.Drawing.Size(372, 305);
            this.glefis1.TabIndex = 0;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 50;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // EFISViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.glefis1);
            this.Name = "EFISViewer";
            this.Size = new System.Drawing.Size(372, 305);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGLUI.GLAI glefis1;
        private System.Windows.Forms.Timer updateTimer;
    }
}
