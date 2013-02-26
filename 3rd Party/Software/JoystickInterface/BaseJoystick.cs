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
