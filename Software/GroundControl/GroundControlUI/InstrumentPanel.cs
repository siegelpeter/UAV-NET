using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using UAVCommons;
namespace GroundControlUI
{

    public partial class InstrumentPanel : UserControl, PersistentData
    {

        DateTime Lastupdate = DateTime.Now;
        GroundControlCore.GroundControlCore mycore = null;
        UAVCommons.UAVStructure AHRS = null;
        UAVCommons.UAVStructure GPS = null;

        int vspeed = 0;
        public InstrumentPanel(GroundControlCore.GroundControlCore mycore)
        {
            InitializeComponent();
            this.mycore = mycore;
          //  mycore.currentUAV.DataArrived += new UAVCommons.UAVBase.DataArrivedHandler(currentUAV_DataArrived);
          //  mycore.currentUAV.uavData.ValueChanged += new UAVCommons.MonitoredDictionary<string, UAVCommons.UAVParameter>.ValueChangedHandler(uavData_ValueChanged);
            LoadDatafromAHRS();
        }


        public string PersistentData
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        void uavData_ValueChanged(UAVCommons.UAVParameter param, bool isremote)
        {
          
            //if (Lastupdate.AddMilliseconds(-100) < DateTime.Now)
            //{
            //    Lastupdate = DateTime.Now;
            //    LoadDatafromAHRS();
            //}
                
        }

        private void LoadDatafromAHRS()
        {
            updateTimer.Enabled = false;
            try
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                UAVParameter speedparam = (UAVParameter)GPS["lbRMCSpeed"];
                UAVParameter altparam = (UAVParameter)GPS["lbGGAAltitude"];
                UAVParameter phiparam = (UAVParameter)AHRS["phi"];
                UAVParameter thetaparam = (UAVParameter)AHRS["theta"];
                UAVParameter psiparam = (UAVParameter)AHRS["psi"];
              
                    if (speedparam != null) if (!speedparam.Value.Equals("N/A")) airSpeedIndicator.SetAirSpeedIndicatorParameters(Convert.ToInt32(speedparam.Value));
                    if ((thetaparam != null) && (phiparam != null)) attitudeIndicator.SetAttitudeIndicatorParameters(Convert.ToDouble(thetaparam.Value), Convert.ToDouble(phiparam.Value));
                // Änderungsrate berechnen
                // Turn Quality berechnen
                // this.vspeed = vspeed + Convert.ToInt32(mycore.currentUAV.uavData["lbGGAAltitude"].Value)*0.9;
                    if ((psiparam != null) && (psiparam != null)) this.Compass.SetHeadingIndicatorParameters(Convert.ToInt32(Convert.ToDouble(psiparam.Value)));
                //  if (mycore.currentUAV.uavData.ContainsKey("yaw")) Compass.SetHeadingIndicatorParameters(Convert.ToInt32(mycore.currentUAV.uavData["yaw"].Value));
           //     if (mycore.currentUAV.uavData.ContainsKey("vspeed")) verticalSpeedIndicator.SetVerticalSpeedIndicatorParameters(Convert.ToInt32(mycore.currentUAV.uavData["vspeed"].Value));
                if (altparam != null) altimeter.SetAlimeterParameters(Convert.ToInt32(Convert.ToDouble(altparam.Value)));
            //    if (mycore.currentUAV.uavData.ContainsKey("turnrate") && mycore.currentUAV.uavData.ContainsKey("turnquality")) turnCoordinator.SetTurnCoordinatorParameters(Convert.ToSingle(mycore.currentUAV.uavData["turnrate"].Value), Convert.ToSingle(mycore.currentUAV.uavData["turnquality"].Value));
                this.Invalidate();
                Console.WriteLine("time update:"+watch.ElapsedMilliseconds);
                watch.Stop();
            }
            catch (Exception ex) { 
            
            }
            updateTimer.Enabled = true;
            }

        void currentUAV_DataArrived(UAVCommons.CommunicationEndpoint source, UAVCommons.UAVParameter arg)
        {
            LoadDatafromAHRS();
        }

        private void turnCoordinator_Load(object sender, EventArgs e)
        {

        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (GPS == null)
            {
                try
                {
                    foreach (var item in mycore.currentUAV.uavData)
                    {
                        if ((item.Value is UAVCommons.UAVStructure) && (((UAVCommons.UAVStructure)item.Value).values.ContainsKey("lbRMCPositionLatitude")))
                        {
                            this.GPS = (UAVCommons.UAVStructure)item.Value;
                            return;
                        }

                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (AHRS == null)
            {
                try
                {
                    foreach (var item in mycore.currentUAV.uavData)
                    {
                        if ((item.Value is UAVCommons.UAVStructure) && (((UAVCommons.UAVStructure)item.Value).values.ContainsKey("phi")))
                        {
                            this.AHRS = (UAVCommons.UAVStructure)item.Value;
                            return;
                        }

                    }
                }
                catch (Exception ex)
                {
                }
            }

            if ((AHRS != null) && (GPS != null))
            {
                LoadDatafromAHRS();
            }
        }
    }
}
