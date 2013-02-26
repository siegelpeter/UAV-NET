namespace GroundControlUI
{
    partial class MapView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapView));
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.neuerWegpunktToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wegpunktBearbeitenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wegpunktLöschenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.btn_centeronuav = new System.Windows.Forms.ToolStripButton();
            this.btn_free = new System.Windows.Forms.ToolStripButton();
            this.lbl_hasfix = new System.Windows.Forms.ToolStripLabel();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gMapControl1
            // 
            this.gMapControl1.AutoScroll = true;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = false;
            this.gMapControl1.CausesValidation = false;
            this.gMapControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(0, 25);
            this.gMapControl1.MapType = GMap.NET.MapType.OpenStreetMap;
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 25;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(505, 273);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 5D;
            this.gMapControl1.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl1_OnMarkerClick);
            this.gMapControl1.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.gMapControl1_OnMarkerEnter);
            this.gMapControl1.OnMarkerLeave += new GMap.NET.WindowsForms.MarkerLeave(this.gMapControl1_OnMarkerLeave);
            this.gMapControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseClick);
            this.gMapControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseMove);
            this.gMapControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseUp);
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
            this.neuerWegpunktToolStripMenuItem.Click += new System.EventHandler(this.neuerWegpunktToolStripMenuItem_Click);
            // 
            // wegpunktBearbeitenToolStripMenuItem
            // 
            this.wegpunktBearbeitenToolStripMenuItem.Name = "wegpunktBearbeitenToolStripMenuItem";
            this.wegpunktBearbeitenToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.wegpunktBearbeitenToolStripMenuItem.Text = "Wegpunkt Bearbeiten";
            this.wegpunktBearbeitenToolStripMenuItem.Click += new System.EventHandler(this.wegpunktBearbeitenToolStripMenuItem_Click);
            // 
            // wegpunktLöschenToolStripMenuItem
            // 
            this.wegpunktLöschenToolStripMenuItem.Name = "wegpunktLöschenToolStripMenuItem";
            this.wegpunktLöschenToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.wegpunktLöschenToolStripMenuItem.Text = "Wegpunkt Löschen";
            this.wegpunktLöschenToolStripMenuItem.Visible = false;
            this.wegpunktLöschenToolStripMenuItem.Click += new System.EventHandler(this.wegpunktLöschenToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.btn_centeronuav,
            this.btn_free,
            this.lbl_hasfix});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(505, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Route auf UAV Übertragen";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // btn_centeronuav
            // 
            this.btn_centeronuav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_centeronuav.Image = ((System.Drawing.Image)(resources.GetObject("btn_centeronuav.Image")));
            this.btn_centeronuav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_centeronuav.Name = "btn_centeronuav";
            this.btn_centeronuav.Size = new System.Drawing.Size(23, 22);
            this.btn_centeronuav.Text = "Auf UAV Ausrichen";
            this.btn_centeronuav.Click += new System.EventHandler(this.btn_centeronuav_Click);
            // 
            // btn_free
            // 
            this.btn_free.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_free.Image = ((System.Drawing.Image)(resources.GetObject("btn_free.Image")));
            this.btn_free.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_free.Name = "btn_free";
            this.btn_free.Size = new System.Drawing.Size(23, 22);
            this.btn_free.Text = "Verschieben";
            this.btn_free.Click += new System.EventHandler(this.btn_free_Click);
            // 
            // lbl_hasfix
            // 
            this.lbl_hasfix.Name = "lbl_hasfix";
            this.lbl_hasfix.Size = new System.Drawing.Size(39, 22);
            this.lbl_hasfix.Text = "Hasfix";
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 250;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(505, 298);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem neuerWegpunktToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wegpunktLöschenToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem wegpunktBearbeitenToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btn_centeronuav;
        private System.Windows.Forms.ToolStripButton btn_free;
        private System.Windows.Forms.ToolStripLabel lbl_hasfix;
        private System.Windows.Forms.Timer updateTimer;
    }
}
