
namespace GroundControlUI
{
    partial class _3DViewer
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
            this._3DModelViewer1 = new OpenGLUI._3DModelViewer();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // _3DModelViewer1
            // 
            this._3DModelViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._3DModelViewer1.Location = new System.Drawing.Point(0, 0);
            this._3DModelViewer1.Name = "_3DModelViewer1";
            this._3DModelViewer1.Size = new System.Drawing.Size(349, 256);
            this._3DModelViewer1.TabIndex = 0;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 50;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // _3DViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._3DModelViewer1);
            this.Name = "_3DViewer";
            this.Size = new System.Drawing.Size(349, 256);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGLUI._3DModelViewer _3DModelViewer1;
        private System.Windows.Forms.Timer updateTimer;

    }
}
