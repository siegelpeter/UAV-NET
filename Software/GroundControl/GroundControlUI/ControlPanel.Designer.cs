namespace GroundControlUI
{
    partial class ControlPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlPanel));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_Choose = new System.Windows.Forms.ToolStripButton();
            this.btn_all = new System.Windows.Forms.ToolStripButton();
            this.cb_count = new System.Windows.Forms.ToolStripComboBox();
            this.ParameterPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Choose,
            this.btn_all,
            this.cb_count});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(351, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_Choose
            // 
            this.btn_Choose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_Choose.Image = ((System.Drawing.Image)(resources.GetObject("btn_Choose.Image")));
            this.btn_Choose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Choose.Name = "btn_Choose";
            this.btn_Choose.Size = new System.Drawing.Size(23, 22);
            this.btn_Choose.Text = "Choose Properties";
            this.btn_Choose.Click += new System.EventHandler(this.btn_Choose_Click);
            // 
            // btn_all
            // 
            this.btn_all.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_all.Image = ((System.Drawing.Image)(resources.GetObject("btn_all.Image")));
            this.btn_all.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_all.Name = "btn_all";
            this.btn_all.Size = new System.Drawing.Size(23, 22);
            this.btn_all.Text = "toolStripButton1";
            this.btn_all.ToolTipText = "Display All";
            this.btn_all.Click += new System.EventHandler(this.btn_all_Click);
            // 
            // cb_count
            // 
            this.cb_count.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cb_count.Name = "cb_count";
            this.cb_count.Size = new System.Drawing.Size(121, 25);
            this.cb_count.TextChanged += new System.EventHandler(this.cb_count_TextChanged);
            // 
            // ParameterPanel
            // 
            this.ParameterPanel.AllowDrop = true;
            this.ParameterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ParameterPanel.Location = new System.Drawing.Point(0, 25);
            this.ParameterPanel.Name = "ParameterPanel";
            this.ParameterPanel.Size = new System.Drawing.Size(351, 182);
            this.ParameterPanel.TabIndex = 3;
            this.ParameterPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.ParameterPanel_DragDrop);
            this.ParameterPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.ParameterPanel_DragEnter);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Interval = 50;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ParameterPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(351, 207);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_Choose;
        private System.Windows.Forms.ToolStripButton btn_all;
        private System.Windows.Forms.ToolStripComboBox cb_count;
        private System.Windows.Forms.FlowLayoutPanel ParameterPanel;
        private System.Windows.Forms.Timer UpdateTimer;
    }
}
