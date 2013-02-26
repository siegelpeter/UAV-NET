namespace GroundControlUI
{
    partial class InstrumentPanel
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
            this.verticalSpeedIndicator = new AvionicsInstrumentControlDemo.VerticalSpeedIndicatorInstrumentControl();
            this.altimeter = new AvionicsInstrumentControlDemo.AltimeterInstrumentControl();
            this.turnCoordinator = new AvionicsInstrumentControlDemo.TurnCoordinatorInstrumentControl();
            this.Compass = new AvionicsInstrumentControlDemo.HeadingIndicatorInstrumentControl();
            this.attitudeIndicator = new AvionicsInstrumentControlDemo.AttitudeIndicatorInstrumentControl();
            this.airSpeedIndicator = new AvionicsInstrumentControlDemo.AirSpeedIndicatorInstrumentControl();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // verticalSpeedIndicator
            // 
            this.verticalSpeedIndicator.Location = new System.Drawing.Point(317, 160);
            this.verticalSpeedIndicator.Name = "verticalSpeedIndicator";
            this.verticalSpeedIndicator.Size = new System.Drawing.Size(150, 150);
            this.verticalSpeedIndicator.TabIndex = 5;
            // 
            // altimeter
            // 
            this.altimeter.Location = new System.Drawing.Point(317, 3);
            this.altimeter.Name = "altimeter";
            this.altimeter.Size = new System.Drawing.Size(150, 150);
            this.altimeter.TabIndex = 4;
            // 
            // turnCoordinator
            // 
            this.turnCoordinator.Location = new System.Drawing.Point(3, 159);
            this.turnCoordinator.Name = "turnCoordinator";
            this.turnCoordinator.Size = new System.Drawing.Size(150, 150);
            this.turnCoordinator.TabIndex = 3;
            this.turnCoordinator.Load += new System.EventHandler(this.turnCoordinator_Load);
            // 
            // Compass
            // 
            this.Compass.Location = new System.Drawing.Point(160, 160);
            this.Compass.Name = "Compass";
            this.Compass.Size = new System.Drawing.Size(150, 150);
            this.Compass.TabIndex = 2;
            // 
            // attitudeIndicator
            // 
            this.attitudeIndicator.Location = new System.Drawing.Point(160, 4);
            this.attitudeIndicator.Name = "attitudeIndicator";
            this.attitudeIndicator.Size = new System.Drawing.Size(150, 150);
            this.attitudeIndicator.TabIndex = 1;
            // 
            // airSpeedIndicator
            // 
            this.airSpeedIndicator.Location = new System.Drawing.Point(3, 3);
            this.airSpeedIndicator.Name = "airSpeedIndicator";
            this.airSpeedIndicator.Size = new System.Drawing.Size(150, 150);
            this.airSpeedIndicator.TabIndex = 0;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 50;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // InstrumentPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.verticalSpeedIndicator);
            this.Controls.Add(this.altimeter);
            this.Controls.Add(this.turnCoordinator);
            this.Controls.Add(this.Compass);
            this.Controls.Add(this.attitudeIndicator);
            this.Controls.Add(this.airSpeedIndicator);
            this.MinimumSize = new System.Drawing.Size(473, 316);
            this.Name = "InstrumentPanel";
            this.Size = new System.Drawing.Size(473, 316);
            this.ResumeLayout(false);

        }

        #endregion

        private AvionicsInstrumentControlDemo.AirSpeedIndicatorInstrumentControl airSpeedIndicator;
        private AvionicsInstrumentControlDemo.AttitudeIndicatorInstrumentControl attitudeIndicator;
        private AvionicsInstrumentControlDemo.HeadingIndicatorInstrumentControl Compass;
        private AvionicsInstrumentControlDemo.TurnCoordinatorInstrumentControl turnCoordinator;
        private AvionicsInstrumentControlDemo.AltimeterInstrumentControl altimeter;
        private AvionicsInstrumentControlDemo.VerticalSpeedIndicatorInstrumentControl verticalSpeedIndicator;
        private System.Windows.Forms.Timer updateTimer;
    }
}
