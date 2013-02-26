using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenGLUI;
using UAVCommons;

namespace GroundControlUI
{
    public partial class _3DViewer : UserControl
    {
        private UAVCommons.UAVBase uav;
          DateTime Lastupdate = DateTime.Now;
        UAVBase mycore = null;
        UAVCommons.UAVStructure AHRS = null;
        UAVCommons.UAVStructure GPS = null;

  

        public _3DViewer(UAVCommons.UAVBase uAVBase)
        {
            this.mycore = uAVBase;
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
                if ((thetaparam != null) && (phiparam != null)) this._3DModelViewer1.SetRotation(Convert.ToInt32(thetaparam.DoubleValue*-1), Convert.ToInt32(psiparam.DoubleValue), Convert.ToInt32(phiparam.DoubleValue));// attitudeIndicator.SetAttitudeIndicatorParameters(Convert.ToDouble(thetaparam.Value), Convert.ToDouble(phiparam.Value));
                // Änderungsrate berechnen
                // Turn Quality berechnen
                // this.vspeed = vspeed + Convert.ToInt32(mycore.currentUAV.uavData["lbGGAAltitude"].Value)*0.9;);
                //    if ((psiparam != null) && (psiparam != null)) this.Compass.SetHeadingIndicatorParameters(Convert.ToInt32(Convert.ToDouble(psiparam.Value)));
                _3DModelViewer1.Refresh();
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
           

            if (AHRS == null)
            {
                try
                {
                    foreach (var item in mycore.uavData)
                    {
                        if ((item.Value is UAVCommons.UAVStructure) && (((UAVCommons.UAVStructure)item.Value).values.ContainsKey("phi")))
                        {
                            this.AHRS = (UAVCommons.UAVStructure)item.Value;
                            break;
                        }

                    }
                }
                catch (Exception ex)
                {
                }
            }

            if ((AHRS != null))
            {
                if (ParentForm.Text.Contains("Demo"))
                {
                    ParentForm.Text = ParentForm.Text.Replace("Demo Mode", "");
                }
            LoadDatafromAHRS();
            }else
            {
                if (!ParentForm.Text.Contains("Demo"))
                {
                
                this.ParentForm.Text += "Demo Mode";
            }
            _3DModelViewer1.UpdateDemo(true);
            }
        }
    }
    }

