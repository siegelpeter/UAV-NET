namespace GroundControlUI
{
    partial class LogViewer
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
            this.logbox = new System.Windows.Forms.ListBox();
            this.updater = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // logbox
            // 
            this.logbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logbox.FormattingEnabled = true;
            this.logbox.Location = new System.Drawing.Point(0, 0);
            this.logbox.Name = "logbox";
            this.logbox.Size = new System.Drawing.Size(522, 164);
            this.logbox.TabIndex = 0;
            this.logbox.SizeChanged += new System.EventHandler(this.logbox_SizeChanged);
            // 
            // updater
            // 
            this.updater.Enabled = true;
            this.updater.Interval = 500;
            this.updater.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logbox);
            this.Name = "LogViewer";
            this.Size = new System.Drawing.Size(522, 164);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox logbox;
        private System.Windows.Forms.Timer updater;
    }
}
