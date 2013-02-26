using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightControlCommons
{
    class UAVPSerialPPM:UAVCommons.UAVStructure
    {
        SerialPort port1 = new SerialPort("Com1", 19200);
        public UAVPSerialPPM()
        {
        port1.NewData+=new SerialPort.NewDataRowHandler(port1_NewData);
        
        }

        void  port1_NewData(object sender, SerialPort.DataEventArgs e)
        {
 
        }
    }
}
