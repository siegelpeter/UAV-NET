/******************************************************************************
 * C# Joystick Library - Copyright (c) 2006 Mark Harris - MarkH@rris.com.au
 ******************************************************************************
 * You may use this library in your application, however please do give credit
 * to me for writing it and supplying it. If you modify this library you must
 * leave this notice at the top of this file. I'd love to see any changes you
 * do make, so please email them to me :)
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
#if windows 
using Microsoft.DirectX.DirectInput;
#endif 
using System.Diagnostics;
using System.Threading;
namespace JoystickInterface
{
    /// <summary>
    /// Class to interface with a joystick device.
    /// </summary>
    public class Joystick : BaseJoystick
    {
#if windows
        private Device joystickDevice;
        private IntPtr hWnd;
        private JoystickState state;
#endif 

        private string[] systemJoysticks;
        private int axisCount;
        private int axisA = -1;
        private int axisB = -1;
        private int axisC = -1;
        private int axisD = -1;
        private int axisE = -1;
        private int axisF = -1;

        Timer updater = null;

        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="window_handle">Handle of the window which the joystick will be "attached" to.</param>
        public Joystick(IntPtr window_handle)
        {
#if windows
         
            hWnd = window_handle;
            axisA = -1;
            axisB = -1;
            axisC = -1;
            axisD = -1;
            axisE = -1;
            axisF = -1;
            axisCount = 0;

            string[] stick = FindJoysticks();
            AcquireJoystick(stick[0]);
#endif
        }

        private void Poll()
        {
       #if windows
         
            try
            {
                // poll the joystick
                joystickDevice.Poll();
                // update the joystick state field
                state = joystickDevice.CurrentJoystickState;
            }
            catch (Exception err)
            {
                // we probably lost connection to the joystick
                // was it unplugged or locked by another application?
                Debug.WriteLine("Poll()");
                Debug.WriteLine(err.Message);
                Debug.WriteLine(err.StackTrace);
            }
#endif
            }

        /// <summary>
        /// Retrieves a list of joysticks attached to the computer.
        /// </summary>
        /// <example>
        /// [C#]
        /// <code>
        /// JoystickInterface.Joystick jst = new JoystickInterface.Joystick(this.Handle);
        /// string[] sticks = jst.FindJoysticks();
        /// </code>
        /// </example>
        /// <returns>A list of joysticks as an array of strings.</returns>
        public string[] FindJoysticks()
        {
            systemJoysticks = null;
#if windows
         
            try
            {
                // Find all the GameControl devices that are attached.
                DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

                // check that we have at least one device.
                if (gameControllerList.Count > 0)
                {
                    systemJoysticks = new string[gameControllerList.Count];
                    int i = 0;
                    // loop through the devices.
                    foreach (DeviceInstance deviceInstance in gameControllerList)
                    {
                        // create a device from this controller so we can retrieve info.
                        joystickDevice = new Device(deviceInstance.InstanceGuid);
                        joystickDevice.SetCooperativeLevel(hWnd,
                            CooperativeLevelFlags.Background |
                            CooperativeLevelFlags.NonExclusive);

                        systemJoysticks[i] = joystickDevice.DeviceInformation.InstanceName;

                        i++;
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("FindJoysticks()");
                Debug.WriteLine(err.Message);
                Debug.WriteLine(err.StackTrace);
            }
#endif
            return systemJoysticks;
        }

        /// <summary>
        /// Acquire the named joystick. You can find this joystick through the <see cref="FindJoysticks"/> method.
        /// </summary>
        /// <param name="name">Name of the joystick.</param>
        /// <returns>The success of the connection.</returns>
        public bool AcquireJoystick(string name)
        {
            #if windows
            try
            {
                DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
                int i = 0;
                bool found = false;
                // loop through the devices.
                foreach (DeviceInstance deviceInstance in gameControllerList)
                {
                    if (deviceInstance.InstanceName == name)
                    {
                        found = true;
                        // create a device from this controller so we can retrieve info.
                        joystickDevice = new Device(deviceInstance.InstanceGuid);
                        joystickDevice.SetCooperativeLevel(hWnd,
                            CooperativeLevelFlags.Background |
                            CooperativeLevelFlags.NonExclusive);
                        
                        break;
                    }

                    i++;
                }

                if (!found)
                    return false;

                // Tell DirectX that this is a Joystick.
                joystickDevice.SetDataFormat(DeviceDataFormat.Joystick);

                // Finally, acquire the device.
                joystickDevice.Acquire();

                // How many axes?
                // Find the capabilities of the joystick
                DeviceCaps cps = joystickDevice.Caps;
                Debug.WriteLine("Joystick Axis: " + cps.NumberAxes);
                Debug.WriteLine("Joystick Buttons: " + cps.NumberButtons);
                
                axisCount = cps.NumberAxes;

                for (int j = 0; j < axisCount+3; j++) {
                    this.Axis.Add(0);
                }

                updater = new Timer(update, null, 0, 10);
            }
            catch (Exception err)
            {
                Debug.WriteLine("FindJoysticks()");
                Debug.WriteLine(err.Message);
                Debug.WriteLine(err.StackTrace);
                return false;
            }
#endif
            return true;
        }

        /// <summary>
        /// Unaquire a joystick releasing it back to the system.
        /// </summary>
        public void ReleaseJoystick()
        {
#if windows
         
            joystickDevice.Unacquire();
#endif
            }

        public void update(object state) {
            updater.Change(Timeout.Infinite, Timeout.Infinite);
            UpdateStatus();
            updater.Change(10,10);
        }

        /// <summary>
        /// Update the properties of button and axis positions.
        /// </summary>
        public void UpdateStatus()
        {
#if windows
            Poll();

            int[] extraAxis = state.GetSlider();
            //Rz Rx X Y Axis1 Axis2
          
            Axis[0] = state.X;
            Axis[1] = state.Y;
            Axis[2] = state.Rz;
            Axis[3] = state.Rx;
            Axis[4] = extraAxis[0];
            Axis[5] = extraAxis[1];

            UpdateStatus(0);
            UpdateStatus(1);
            UpdateStatus(2);
            UpdateStatus(3);
            UpdateStatus(4);
            UpdateStatus(5);

            // not using buttons, so don't take the tiny amount of time it takes to get/parse
            //byte[] jsButtons = state.GetButtons();
            //buttons = new bool[jsButtons.Length];

            //int i = 0;
            //foreach (byte button in jsButtons)
            //{
            //    buttons[i] = button >= 128;
            //    i++;
            //}
#endif
        }
    }
}
