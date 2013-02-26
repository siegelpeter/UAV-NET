using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UAVCommons;

namespace GroundControlUI
{
    public partial class EFISViewer : UserControl
    {
        DateTime Lastupdate = DateTime.Now;
        GroundControlCore.GroundControlCore mycore = null;
        UAVCommons.UAVStructure AHRS = null;
        UAVCommons.UAVStructure GPS = null;
        private UAVCommons.UAVBase uAVBase;

        public EFISViewer()
        {
            InitializeComponent();
        }

        public EFISViewer(GroundControlCore.GroundControlCore uAVBase)
        {
            // TODO: Complete member initialization
            this.mycore = uAVBase;
            this.uAVBase = uAVBase.currentUAV;
            InitializeComponent();
        }

     
        
        private void LoadDatafromAHRS()
        {
            updateTimer.Enabled = false;
            try
            {
           //     System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //    watch.Start();

                UAVStructure phiparam = (UAVStructure)AHRS["phi"];
                UAVStructure thetaparam = (UAVStructure)AHRS["theta"];
                UAVStructure psiparam = (UAVStructure)AHRS["psi"];
              
                  //  if (speedparam != null) if (!speedparam.Value.Equals("N/A")) airSpeedIndicator.SetAirSpeedIndicatorParameters(Convert.ToInt32(speedparam.Value));
                if ((thetaparam != null) && (phiparam != null) && (psiparam != null))
                {
                    this.glefis1.ahrs.heading = Convert.ToInt32(psiparam.DoubleValue);
                    this.glefis1.ahrs.pitch = Convert.ToInt32(thetaparam.DoubleValue);
                    this.glefis1.ahrs.bank =  Convert.ToInt32(phiparam.DoubleValue)*-1;
                  
                        // attitudeIndicator.SetAttitudeIndicatorParameters(Convert.ToDouble(thetaparam.Value), Convert.ToDouble(phiparam.Value));
                }


                // Änderungsrate berechnen
                // Turn Quality berechnen
                // this.vspeed = vspeed + Convert.ToInt32(mycore.currentUAV.uavData["lbGGAAltitude"].Value)*0.9;);
                //    if ((psiparam != null) && (psiparam != null)) this.Compass.SetHeadingIndicatorParameters(Convert.ToInt32(Convert.ToDouble(psiparam.Value)));
                this.glefis1.Refresh();
                //  Console.WriteLine("time update:"+watch.ElapsedMilliseconds);
         //       watch.Stop();
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

            if ((AHRS != null))
            {
                LoadDatafromAHRS();
            }
        }

       
    }

    }

