namespace GroundControlUI
{
    partial class PropertyWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

      

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyWindow));
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_Choose = new System.Windows.Forms.ToolStripButton();
            this.btn_all = new System.Windows.Forms.ToolStripButton();
            this.btn_update = new System.Windows.Forms.ToolStripButton();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(0, 28);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(230, 265);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.Click += new System.EventHandler(this.propertyGrid_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Choose,
            this.btn_all,
            this.btn_update});
            this.toolStrip1.Location = new System.Drawing.Point(0, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(230, 25);
            this.toolStrip1.TabIndex = 1;
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
            // btn_update
            // 
            this.btn_update.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btn_update.Image = ((System.Drawing.Image)(resources.GetObject("btn_update.Image")));
            this.btn_update.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(49, 22);
            this.btn_update.Text = "Update";
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // PropertyWindow
            // 
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PropertyWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.Size = new System.Drawing.Size(230, 296);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_Choose;
        private System.Windows.Forms.ToolStripButton btn_all;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ToolStripButton btn_update;
    }
}