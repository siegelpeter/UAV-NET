using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
namespace FlightControlCommons.components
{
          [Serializable]
    public class BaseAutoPilot : BasePilot
    {
        public UAVCommons.UAVStructure PerformanceData = null;
        public UAVCommons.UAVStructure ahrs = null;
        public UAVCommons.UAVStructure gps = null;
        public UAVCommons.UAVStructure windFahne = null;
        public UAVCommons.UAVStructure Pitot = null; // Emulate using GS from GPS

        /// <summary>
        /// Pids for all axis
        /// </summary>
        public PIDLibrary.ParameterPID phiPid = null;
        public PIDLibrary.ParameterPID psiPid = null;
        public PIDLibrary.ParameterPID thetaPid = null;
        BaseNavigation navigation = null;

        public BaseAutoPilot(string name, BaseNavigation navigation, UAVCommons.UAVStructure mygps, UAVCommons.UAVStructure myahrs, UAVCommons.UAVStructure performancedata)
            : base(name)
        {

            this.navigation = navigation;
            this.gps = mygps;
            this.ahrs = myahrs;
            this.PerformanceData = performancedata;

            //Pid Config
            // this.values.Add(new UAVCommons.UAVParameter("phi_P_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("psi_P_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("theta_P_gain", 1));

            //   this.values.Add(new UAVCommons.UAVParameter("phi_I_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("psi_I_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("theta_I_gain", 1));

            //   this.values.Add(new UAVCommons.UAVParameter("phi_D_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("psi_D_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("theta_D_gain", 1));

            Console.WriteLine("AutoPilot: Set Performance DataPoints");
            //TargetVector from Navigation
            this.values.Add(new UAVCommons.UAVParameter("bankangle", 1, PerformanceData["MaxBankRight"].Value, PerformanceData["MaxBankLeft"].Value));
           this.values.Add(new UAVCommons.UAVParameter("theta_out", 1, PerformanceData["MaxDescendAngle"].Value, PerformanceData["MaxClimbAngle"].Value));
            
            PerformanceData.ValueChanged += new UAVStructure.ValueChangedHandler(PerformanceData_ValueChanged);

            UAVParameter center = new UAVParameter("Center", 0, 180, -180);
            //Achtung muss noch unterscheiden zwischen drehung nach link oder rechts  ahrs.values["psi"], values["bankangle"],navigation.values["psi"]
            // vs speed muss noch berechnet werden und als eingangswert statt altitutude  ned so wichtig
            Console.WriteLine("AutoPilot: Init Bank PID");
            psiPid = new PIDLibrary.ParameterPID(values["psi_P_gain"], values["psi_I_gain"], values["psi_D_gain"], center, values["bankangle"], new PIDLibrary.GetDouble(psiPV), new PIDLibrary.GetDouble(psiSP), new PIDLibrary.SetDouble(Setpsi_out));
            Console.WriteLine("AutoPilot: Init ClimbDescend PID");
            if (((UAVStructure)gps).values.ContainsKey("lbGGAAltitude")) Console.WriteLine("Autopilot: Gps Altitude found");
            
            thetaPid = new PIDLibrary.ParameterPID(values["theta_P_gain"], values["theta_I_gain"], values["theta_D_gain"], gps["lbGGAAltitude"], values["theta_out"], navigation.values["TargetAltitude"]);
            
            //   PID QuerachsenPID = new PID(1, 1, 1, 90, -90, 2000, 1000, new GetDouble(GetPV), new GetDouble(GetSP),new SetDouble( SetResult));
            Console.WriteLine("AutoPilot: PID Enable");
           // phiPid.Enable();
     
        }

        void PerformanceData_ValueChanged(UAVParameter param, bool isremote)
        {
            values["bankangle"].Min = PerformanceData["MaxBankLeft"].Value;
            values["bankangle"].Max = PerformanceData["MaxBankRight"].Value;
            values["theta_out"].Max = PerformanceData["MaxClimbAngle"].Value;
            values["theta_out"].Min = PerformanceData["MaxDescendAngle"].Value;

        }

        /// <summary>
        /// PV = 0, imagine the front of the aircraft is 0 degrees, 
        /// </summary>
        /// <returns></returns>
        private Double psiPV()
        {

            return 0;
        }

        /// <summary>
        /// Give Navigation - curent psi as input for pid
        /// </summary>
        /// <returns></returns>
        private Double psiSP()
        {
            return navigation.values["TargetPsi"].DoubleValue - (ahrs.values["psi"].DoubleValue);
        }
        /// <summary>
        /// Set Result on PID Output
        /// </summary>
        /// <param name="value">Result from Pid</param>
        private void Setpsi_out(Double value)
        {
            values["bankangle"].Value = value;
        }
        public virtual void ahrs_ValueChanged(UAVCommons.UAVParameter param, bool isremote)
        {
            //lastphi = Convert.ToDouble(ahrs.values["phi"]);
            //lastpsi = Convert.ToDouble(ahrs.values["psi"]);
            //lasttheta = Convert.ToDouble(ahrs.values["theta"]);
        }




    }
}
