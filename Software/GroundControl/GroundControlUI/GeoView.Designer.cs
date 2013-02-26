namespace GroundControlUI
{
    partial class GeoView
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.neuerWegpunktToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wegpunktBearbeitenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wegpunktLöschenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neuerWegpunktToolStripMenuItem,
            this.wegpunktBearbeitenToolStripMenuItem,
            this.wegpunktLöschenToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 70);
            // 
            // neuerWegpunktToolStripMenuItem
            // 
            this.neuerWegpunktToolStripMenuItem.Name = "neuerWegpunktToolStripMenuItem";
            this.neuerWegpunktToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.neuerWegpunktToolStripMenuItem.Text = "Neuer Wegpunkt";
            // 
            // wegpunktBearbeitenToolStripMenuItem
            // 
            this.wegpunktBearbeitenToolStripMenuItem.Name = "wegpunktBearbeitenToolStripMenuItem";
            this.wegpunktBearbeitenToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.wegpunktBearbeitenToolStripMenuItem.Text = "Wegpunkt Bearbeiten";
            // 
            // wegpunktLöschenToolStripMenuItem
            // 
            this.wegpunktLöschenToolStripMenuItem.Name = "wegpunktLöschenToolStripMenuItem";
            this.wegpunktLöschenToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.wegpunktLöschenToolStripMenuItem.Text = "Wegpunkt Löschen";
            this.wegpunktLöschenToolStripMenuItem.Visible = false;
            // 
            // GeoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "GeoView";
            this.Size = new System.Drawing.Size(227, 202);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem neuerWegpunktToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wegpunktLöschenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wegpunktBearbeitenToolStripMenuItem;
    }
}
