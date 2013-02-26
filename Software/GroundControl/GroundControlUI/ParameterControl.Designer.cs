namespace GroundControlUI
{
    partial class ParameterControl
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
            this.cb_Parameter = new System.Windows.Forms.ComboBox();
            this.btn_output = new System.Windows.Forms.Button();
            this.lbl_value = new System.Windows.Forms.Label();
            this.tb_min = new System.Windows.Forms.TextBox();
            this.tb_max = new System.Windows.Forms.TextBox();
            this.ValueBar = new ColorProgressBar.ColorProgressBar();
            this.valueTracker = new TrackBarDecimals.DecimalTrackbar();
            ((System.ComponentModel.ISupportInitialize)(this.valueTracker)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_Parameter
            // 
            this.cb_Parameter.FormattingEnabled = true;
            this.cb_Parameter.Location = new System.Drawing.Point(4, 4);
            this.cb_Parameter.Name = "cb_Parameter";
            this.cb_Parameter.Size = new System.Drawing.Size(199, 21);
            this.cb_Parameter.TabIndex = 0;
            this.cb_Parameter.DropDown += new System.EventHandler(this.cb_Parameter_DropDown);
            this.cb_Parameter.SelectedIndexChanged += new System.EventHandler(this.cb_Parameter_SelectedIndexChanged);
            // 
            // btn_output
            // 
            this.btn_output.ForeColor = System.Drawing.Color.Red;
            this.btn_output.Location = new System.Drawing.Point(215, 4);
            this.btn_output.Name = "btn_output";
            this.btn_output.Size = new System.Drawing.Size(75, 23);
            this.btn_output.TabIndex = 1;
            this.btn_output.Text = "Ausgabe";
            this.btn_output.UseVisualStyleBackColor = true;
            this.btn_output.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbl_value
            // 
            this.lbl_value.AutoSize = true;
            this.lbl_value.Location = new System.Drawing.Point(530, 41);
            this.lbl_value.Name = "lbl_value";
            this.lbl_value.Size = new System.Drawing.Size(30, 13);
            this.lbl_value.TabIndex = 2;
            this.lbl_value.Text = "Wert";
            // 
            // tb_min
            // 
            this.tb_min.Location = new System.Drawing.Point(296, 34);
            this.tb_min.Name = "tb_min";
            this.tb_min.ReadOnly = true;
            this.tb_min.Size = new System.Drawing.Size(41, 20);
            this.tb_min.TabIndex = 4;
            this.tb_min.Text = "0";
            this.tb_min.TextChanged += new System.EventHandler(this.tb_min_TextChanged);
            // 
            // tb_max
            // 
            this.tb_max.Location = new System.Drawing.Point(716, 38);
            this.tb_max.Name = "tb_max";
            this.tb_max.ReadOnly = true;
            this.tb_max.Size = new System.Drawing.Size(41, 20);
            this.tb_max.TabIndex = 5;
            this.tb_max.Text = "100";
            this.tb_max.TextChanged += new System.EventHandler(this.tb_max_TextChanged);
            // 
            // ValueBar
            // 
            this.ValueBar.BarColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ValueBar.BorderColor = System.Drawing.Color.Black;
            this.ValueBar.FillStyle = ColorProgressBar.ColorProgressBar.FillStyles.Solid;
            this.ValueBar.Location = new System.Drawing.Point(296, 4);
            this.ValueBar.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ValueBar.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ValueBar.Name = "ValueBar";
            this.ValueBar.Size = new System.Drawing.Size(464, 21);
            this.ValueBar.Step = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ValueBar.TabIndex = 7;
            this.ValueBar.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ValueBar.Visible = false;
            // 
            // valueTracker
            // 
            this.valueTracker.Factor = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.valueTracker.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.valueTracker.Location = new System.Drawing.Point(296, 4);
            this.valueTracker.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.valueTracker.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            65536});
            this.valueTracker.Name = "valueTracker";
            this.valueTracker.Size = new System.Drawing.Size(471, 45);
            this.valueTracker.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.valueTracker.TabIndex = 3;
            this.valueTracker.TickFrequency = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.valueTracker.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.valueTracker.Scroll += new System.EventHandler(this.valueTracker_Scroll);
            // 
            // ParameterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ValueBar);
            this.Controls.Add(this.lbl_value);
            this.Controls.Add(this.tb_max);
            this.Controls.Add(this.tb_min);
            this.Controls.Add(this.valueTracker);
            this.Controls.Add(this.btn_output);
            this.Controls.Add(this.cb_Parameter);
            this.DoubleBuffered = true;
            this.Name = "ParameterControl";
            this.Size = new System.Drawing.Size(768, 61);
            ((System.ComponentModel.ISupportInitialize)(this.valueTracker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_Parameter;
        private System.Windows.Forms.Button btn_output;
        private System.Windows.Forms.Label lbl_value;
        private TrackBarDecimals.DecimalTrackbar valueTracker;
        private System.Windows.Forms.TextBox tb_min;
        private System.Windows.Forms.TextBox tb_max;
        private ColorProgressBar.ColorProgressBar ValueBar;
    }
}
