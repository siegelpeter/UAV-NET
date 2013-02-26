namespace GroundControlUI
{
    partial class ConnectionsPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Priorität = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Protokoll = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ClientInfo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ConnectionType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Verbindungsart = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Latenzzeit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnectionState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tog = new System.Windows.Forms.DataGridViewButtonColumn();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cb_Source = new System.Windows.Forms.ToolStripComboBox();
            this.lbl_EventsperSec = new System.Windows.Forms.ToolStripLabel();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Priorität,
            this.Protokoll,
            this.ClientInfo,
            this.ConnectionType,
            this.Verbindungsart,
            this.Latenzzeit,
            this.ConnectionState,
            this.Tog});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(448, 237);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserAddedRow);
            this.dataGridView1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserDeletedRow);
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView1_UserDeletingRow);
            // 
            // Priorität
            // 
            this.Priorität.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Priorität.HeaderText = "Priorität";
            this.Priorität.Name = "Priorität";
            this.Priorität.Width = 67;
            // 
            // Protokoll
            // 
            this.Protokoll.HeaderText = "Protokoll";
            this.Protokoll.Items.AddRange(new object[] {
            "TCP",
            "UDP",
            "Unbekannt"});
            this.Protokoll.Name = "Protokoll";
            // 
            // ClientInfo
            // 
            this.ClientInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ClientInfo.HeaderText = "Endpunkt";
            this.ClientInfo.Items.AddRange(new object[] {
            "Server",
            "Client"});
            this.ClientInfo.Name = "ClientInfo";
            this.ClientInfo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ClientInfo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ClientInfo.Width = 78;
            // 
            // ConnectionType
            // 
            this.ConnectionType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ConnectionType.HeaderText = "Verbindungstyp";
            this.ConnectionType.Items.AddRange(new object[] {
            "Wifi",
            "Gprs3G"});
            this.ConnectionType.Name = "ConnectionType";
            this.ConnectionType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ConnectionType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ConnectionType.Width = 105;
            // 
            // Verbindungsart
            // 
            this.Verbindungsart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Verbindungsart.HeaderText = "Richtung";
            this.Verbindungsart.Items.AddRange(new object[] {
            "Command",
            "Recieve",
            "Send"});
            this.Verbindungsart.Name = "Verbindungsart";
            this.Verbindungsart.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Verbindungsart.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Verbindungsart.Width = 75;
            // 
            // Latenzzeit
            // 
            this.Latenzzeit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Latenzzeit.HeaderText = "Latenzzeit";
            this.Latenzzeit.Name = "Latenzzeit";
            this.Latenzzeit.Width = 80;
            // 
            // ConnectionState
            // 
            this.ConnectionState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.ConnectionState.HeaderText = "Verbindungsstatus";
            this.ConnectionState.Name = "ConnectionState";
            this.ConnectionState.Width = 21;
            // 
            // Tog
            // 
            this.Tog.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Tog.HeaderText = "Trennen";
            this.Tog.Name = "Tog";
            this.Tog.Text = "...";
            this.Tog.UseColumnTextForButtonValue = true;
            this.Tog.Width = 53;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(150, 150);
            this.toolStripContainer1.Location = new System.Drawing.Point(18, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(150, 175);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cb_Source,
            this.lbl_EventsperSec});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(448, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(24, 22);
            this.toolStripLabel1.Text = "Ort";
            // 
            // cb_Source
            // 
            this.cb_Source.Items.AddRange(new object[] {
            "Bodenstation",
            "UAV"});
            this.cb_Source.Name = "cb_Source";
            this.cb_Source.Size = new System.Drawing.Size(121, 25);
            this.cb_Source.SelectedIndexChanged += new System.EventHandler(this.cb_Source_SelectedIndexChanged);
            // 
            // lbl_EventsperSec
            // 
            this.lbl_EventsperSec.Name = "lbl_EventsperSec";
            this.lbl_EventsperSec.Size = new System.Drawing.Size(86, 22);
            this.lbl_EventsperSec.Text = "toolStripLabel2";
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // ConnectionsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "ConnectionsPanel";
            this.Size = new System.Drawing.Size(448, 262);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cb_Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn Priorität;
        private System.Windows.Forms.DataGridViewComboBoxColumn Protokoll;
        private System.Windows.Forms.DataGridViewComboBoxColumn ClientInfo;
        private System.Windows.Forms.DataGridViewComboBoxColumn ConnectionType;
        private System.Windows.Forms.DataGridViewComboBoxColumn Verbindungsart;
        private System.Windows.Forms.DataGridViewTextBoxColumn Latenzzeit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnectionState;
        private System.Windows.Forms.DataGridViewButtonColumn Tog;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ToolStripLabel lbl_EventsperSec;
    }
}
