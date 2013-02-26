using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GroundControl_Win
{
    public class RemoteDevice
    {
        public System.Net.IPAddress target;
        public string username;
        public string password;
     

        public RemoteDevice(string adr, string usr, string pass)
        {
            this.username = usr;
            this.password = pass;
            target = IPAddress.Parse(adr);
        }
    }
}
