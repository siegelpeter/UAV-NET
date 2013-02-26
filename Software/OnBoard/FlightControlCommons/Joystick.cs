#region LICENSE
/*
 * Copyright (C) 2004 David Hudson (jendave@yahoo.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */   //fuck David -was juckt mich des ??
#endregion LICENSE

using System;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if windows
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.Audio;
using SdlDotNet.Core;
#endif
using UAVCommons;

namespace FlightControlCommons
{
    [Serializable]
    public class UAVJoystick : UAVCommons.UAVStructure, IDisposable, IHardwareConnectable
    {
        // JoystickInterface.Joystick
 
        public bool errorOnCreate = false;
        public IntPtr window;
        public  string port;
       [NonSerialized]
        JoystickInterface.BaseJoystick joystick;
        [NonSerialized]
        System.Threading.Timer time1;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Joystick</param>
        /// <param name="port">currently unused</param>
        /// <param name="window">Handle of the Window</param>
        public UAVJoystick(string name, string port, IntPtr window)
            : base(name, "")
        {
            this.window = window;
            this.port = port;
            CreateParameter(); 
        
        }
        /// <summary>
        /// Connects to the Joystick device
        /// </summary>
        public void ConnectHardware()
        {
            Go(port);
			
			Console.WriteLine("Connect Joystick");
        }

        /// <summary>
        /// Initialises the Joystick Values
        /// </summary>
        private void CreateParameter()
        {
            values.Add(new JoyStickParam(joystick,0,"Axis0", 0, 100, -100, 100)); // ?????????was bedeuten die Einträge ?
            values.Add(new JoyStickParam(joystick,1,"Axis1", 0, 100, -100, 100));
            values.Add(new JoyStickParam(joystick,2,"Axis2", 0, 100, -100, 100));
            values.Add(new JoyStickParam(joystick,3,"Axis3", 0, 100, -100, 100));
            values.Add(new JoyStickParam(joystick,4,"Axis4", 0, 100, -100, 100));
            values.Add(new JoyStickParam(joystick,5,"Axis5", 0, 100, -100, 100));
            values.Add(new JoyStickParam(joystick,6,"Axis6", 0, 100, -100, 100));

          //  values.Add(new UAVCommons.UAVParameter("Buttons", 0));

        }

        public void Go(string port)
        {
            JoystickInterface.BaseJoystick.Window = window;
            joystick = JoystickInterface.BaseJoystick.GetJoystickClass();
			if (joystick != null){
            joystick.ValueChanged += new JoystickInterface.BaseJoystick.ValueChangedHandler(joystick_ValueChanged);
           // time1 = new System.Threading.Timer(new TimerCallback(tick), null, 0, 1);
			foreach (UAVParameter param in values.Values){
				if (param is JoyStickParam) ((JoyStickParam)param).RegisterJoyStick(joystick);
				
			}
			}
		    
        }

        void joystick_ValueChanged(int nr)
        {
            ((FlightControlCommons.JoyStickParam)values["Axis" + nr]).Changed();
        }

        /// <summary>
        /// Update the Joystickvalues every x miliseconds
        /// </summary>
        /// <param name="state"></param>
        public void tick(object state)
        {
            joystick.UpdateStatus(0);
			
////Console.WriteLine("Axis0" + Convert.ToSingle(joystick.AxisA) / 32767*100);//
//			if (OldAxisA != joystick.AxisA)
//            {
//               values["Axis0"].DoubleValue = joystick.AxisA/32767*100;
//					//Console.WriteLine("Axis0" + Convert.ToSingle(joystick.AxisA)+" "+values["Axis0"].DoubleValue);
//                OldAxisA = joystick.AxisA;
//            }
//            if (OldAxisB != joystick.AxisB)
//            {
//                values["Axis1"].DoubleValue = joystick.AxisB/32767*100;
//               // values["RawAxis1"].Value = Convert.ToSingle(joystick.AxisB) / 65535;
//                OldAxisB = joystick.AxisB;
//            }
//            if (OldAxisC != joystick.AxisC)
//            {
//
//                values["Axis2"].DoubleValue = joystick.AxisC/32767*100;
//          //      values["RawAxis2"].Value = Convert.ToSingle(joystick.AxisC) / 65535;
//                OldAxisC = joystick.AxisC;
//            }
//            if (OldAxisD != joystick.AxisD)
//            {
//
//                values["Axis3"].DoubleValue = joystick.AxisD/32767*100;
//         //       values["RawAxis3"].Value = 1 - (Convert.ToSingle(joystick.AxisD) / 65535);
//                OldAxisD = joystick.AxisD;
//            }
//
//            if (OldAxisE != joystick.AxisE)
//            {
//
//                values["Axis4"].DoubleValue = joystick.AxisE/32767*100;
//         //       values["RawAxis4"].Value = Convert.ToSingle(joystick.AxisE) / 65535;
//                OldAxisE = joystick.AxisE;
//            }
//
//            if (OldAxisF != joystick.AxisF)
//            {
//
//                values["Axis5"].DoubleValue = joystick.AxisF/32767*100;
//         //       values["RawAxis5"].Value = Convert.ToSingle(joystick.AxisF) / 65535;
//                OldAxisF = joystick.AxisF;
//            }


        }


        #region IDisposable Members

        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    /*   if (this.screen != null)
                       {
                           this.screen.Dispose();
                           this.screen = null;
                       }
                       if (this.cursor != null)
                       {
                           this.cursor.Dispose();
                           this.cursor = null;
                       }*/
                    if (this.joystick != null)
                    {
                        this.joystick.ReleaseJoystick();
                        this.joystick = null;
                    }
                }
                this.disposed = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
       
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        ~UAVJoystick()
        {
            Dispose(false);
        }

        #endregion
    }
}
