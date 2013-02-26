/*
    UAV.NET: a UAV / Robotic control framework for .net
    Copyright (C) 2012  Peter Siegel

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace JoystickInterface
{
    public class BaseJoystick
    {
        public static IntPtr Window;
		public List<double> Axis = new List<double>();
        public delegate void ValueChangedHandler(int nr);

        /// <summary>
        /// The Event ValueChanged is fired when this Parameter has been Updates
        /// </summary>
        public event ValueChangedHandler ValueChanged;


        /// <summary>
        /// Returns true if Running under Linux e.g.: on the UAV 
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static BaseJoystick GetJoystickClass()
        {
			
            if (IsLinux)
            {
                return new LinuxJoyStick();
            }
             return new Joystick(Window);
			return null;
        }

        #region Properties 
        private float axisCount;
        /// <summary>
        /// Number of axes on the joystick.
        /// </summary>
        public float AxisCount
        {
            get { return axisCount; }
        }

//        protected float axisA;
//        /// <summary>
//        /// The first axis on the joystick.
//        /// </summary>
//        public float AxisA
//        {
//            get { return axisA; }
//        }
//
//        protected float axisB;
//        /// <summary>
//        /// The second axis on the joystick.
//        /// </summary>
//        public float AxisB
//        {
//            get { return axisB; }
//        }
//
//        protected float axisC;
//        /// <summary>
//        /// The third axis on the joystick.
//        /// </summary>
//        public float AxisC
//        {
//            get { return axisC; }
//        }
//
//        protected float axisD;
//        /// <summary>
//        /// The fourth axis on the joystick.
//        /// </summary>
//        public float AxisD
//        {
//            get { return axisD; }
//        }
//
//        protected float axisE;
//        /// <summary>
//        /// The fifth axis on the joystick.
//        /// </summary>
//        public float AxisE
//        {
//            get { return axisE; }
//        }
//
//        protected float axisF;
//        /// <summary>
//        /// The sixth axis on the joystick.
//        /// </summary>
//        public float AxisF
//        {
//            get { return axisF; }
//        }
//        protected IntPtr hWnd;

        protected bool[] buttons;
        /// <summary>
        /// Array of buttons availiable on the joystick. This also includes PoV hats.
        /// </summary>
        public bool[] Buttons
        {
            get { return buttons; }
        }


        #endregion
		
		
		
        public virtual void UpdateStatus(int channel)
        {
           if (ValueChanged != null) ValueChanged(channel);
        }

        public virtual  void ReleaseJoystick()
        {
        }
    }
}
