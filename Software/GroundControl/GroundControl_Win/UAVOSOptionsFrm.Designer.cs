namespace GroundControl_Win
{
    partial class UAVOSOptionsFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
   
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tb_autostart = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.tb_settings = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btn_LoadFrom = new System.Windows.Forms.Button();
            this.btn_SaveAs = new System.Windows.Forms.Button();
            this.tabControl2.SuspendLayout();
            this.tb_autostart.SuspendLayout();
            this.tb_settings.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tb_autostart);
            this.tabControl2.Controls.Add(this.tb_settings);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(385, 230);
            this.tabControl2.TabIndex = 4;
            // 
            // tb_autostart
            // 
            this.tb_autostart.Controls.Add(this.button1);
            this.tb_autostart.Controls.Add(this.button2);
            this.tb_autostart.Controls.Add(this.button3);
            this.tb_autostart.Controls.Add(this.checkBox2);
            this.tb_autostart.Location = new System.Drawing.Point(4, 22);
            this.tb_autostart.Name = "tb_autostart";
            this.tb_autostart.Padding = new System.Windows.Forms.Padding(3);
            this.tb_autostart.Size = new System.Drawing.Size(377, 204);
            this.tb_autostart.TabIndex = 0;
            this.tb_autostart.Text = "Autostart";
            this.tb_autostart.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(272, 171);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 27);
            this.button1.TabIndex = 7;
            this.button1.Text = "update";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(6, 30);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Stop FlightControl";
            this.button3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 6);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(142, 17);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "autostarting FlightControl";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // tb_settings
            // 
            this.tb_settings.Controls.Add(this.btn_SaveAs);
            this.tb_settings.Controls.Add(this.btn_LoadFrom);
            this.tb_settings.Controls.Add(this.listBox1);
            this.tb_settings.Location = new System.Drawing.Point(4, 22);
            this.tb_settings.Name = "tb_settings";
            this.tb_settings.Padding = new System.Windows.Forms.Padding(3);
            this.tb_settings.Size = new System.Drawing.Size(377, 204);
            this.tb_settings.TabIndex = 1;
            this.tb_settings.Text = "Onboard Settings";
            this.tb_settings.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(365, 160);
            this.listBox1.TabIndex = 0;
            // 
            // btn_LoadFrom
            // 
            this.btn_LoadFrom.Location = new System.Drawing.Point(120, 172);
            this.btn_LoadFrom.Name = "btn_LoadFrom";
            this.btn_LoadFrom.Size = new System.Drawing.Size(134, 23);
            this.btn_LoadFrom.TabIndex = 1;
            this.btn_LoadFrom.Text = "Load selected Settings";
            this.btn_LoadFrom.UseVisualStyleBackColor = true;
            this.btn_LoadFrom.Click += new System.EventHandler(this.btn_LoadFrom_Click);
            // 
            // btn_SaveAs
            // 
            this.btn_SaveAs.Location = new System.Drawing.Point(260, 172);
            this.btn_SaveAs.Name = "btn_SaveAs";
            this.btn_SaveAs.Size = new System.Drawing.Size(109, 23);
            this.btn_SaveAs.TabIndex = 2;
            this.btn_SaveAs.Text = "Save As";
            this.btn_SaveAs.UseVisualStyleBackColor = true;
            this.btn_SaveAs.Click += new System.EventHandler(this.btn_SaveAs_Click);
            // 
            // UAVOSOptionsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 230);
            this.Controls.Add(this.tabControl2);
            this.Name = "UAVOSOptionsFrm";
            this.Text = "UAV On Board System Control";
            this.tabControl2.ResumeLayout(false);
            this.tb_autostart.ResumeLayout(false);
            this.tb_autostart.PerformLayout();
            this.tb_settings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tb_autostart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TabPage tb_settings;
        private System.Windows.Forms.Button btn_SaveAs;
        private System.Windows.Forms.Button btn_LoadFrom;
        private System.Windows.Forms.ListBox listBox1;

    }
}