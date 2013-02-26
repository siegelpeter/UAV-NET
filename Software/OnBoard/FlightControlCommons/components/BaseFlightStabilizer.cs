using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
namespace FlightControlCommons.components
{
    /// <summary>
    /// FluglageRegler
    /// </summary>
    [Serializable]
    public class BaseFlightStabilizer : UAVStructure
    {
        /// <summary>
        /// AHRS Object associated with
        /// </summary>
        public UAVStructure ahrs = null;
        
        /// <summary>
        /// Autopilot Input 
        /// </summary>
        public UAVStructure ap = null;
        
        /// <summary>
        /// Servo Output
        /// </summary>
        public UAVParameter phi_out = null;
        public UAVParameter theta_out = null;
        public UAVParameter psi_out = null;

        /// <summary>
        /// Pids for all axis
        /// </summary>
        public PIDLibrary.ParameterPID phiPid = null;
        public PIDLibrary.ParameterPID psiPid = null;
        public PIDLibrary.ParameterPID thetaPid = null;

       


        public BaseFlightStabilizer(string name, UAVStructure myAhrs, UAVStructure ap, UAVParameter phi_out,UAVParameter psi_out,UAVParameter theta_out)
            : base(name, null)
        {
            this.ahrs = myAhrs;
            this.ap = ap;

            this.phi_out = phi_out;
            this.psi_out = psi_out;
            this.theta_out = theta_out;
          
            this.values.Add(new UAVCommons.UAVParameter("phi_P_gain", 1));
        //    this.values.Add(new UAVCommons.UAVParameter("psi_P_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("theta_P_gain", 1));

            this.values.Add(new UAVCommons.UAVParameter("phi_I_gain", 1));
          //  this.values.Add(new UAVCommons.UAVParameter("psi_I_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("theta_I_gain", 1));

            this.values.Add(new UAVCommons.UAVParameter("phi_D_gain", 1));
         //   this.values.Add(new UAVCommons.UAVParameter("psi_D_gain", 1));
            this.values.Add(new UAVCommons.UAVParameter("theta_D_gain", 1));


            phiPid = new PIDLibrary.ParameterPID(values["phi_P_gain"], values["phi_I_gain"], values["phi_D_gain"], this.ahrs["phi"], phi_out, ap.values["bankangle"]);
            //   psiPid = new PIDLibrary.ParameterPID(values["psi_P_gain"], values["psi_I_gain"], values["psi_D_gain"], this.ahrs["psi"], psi_out, ap.values["psi_out"]);//should only correct drift
            thetaPid = new PIDLibrary.ParameterPID(values["theta_P_gain"], values["theta_I_gain"], values["theta_D_gain"], this.ahrs["theta"], theta_out, ap.values["theta_out"]);

               
               //   PID QuerachsenPID = new PID(1, 1, 1, 90, -90, 2000, 1000, new GetDouble(GetPV), new GetDouble(GetSP),new SetDouble( SetResult));
             
            phiPid.Enable();
         //   psiPid.Enable();
            thetaPid.Enable();
        
        }

  

        /// <summary>
        /// Use this Module to 
        /// </summary>
      /*  public void ComputeMixers() {
            for (int i = 0; i < mixers.Count; i++) {
                mixers[i].Compute();
            }
        }
        */
    }
}
