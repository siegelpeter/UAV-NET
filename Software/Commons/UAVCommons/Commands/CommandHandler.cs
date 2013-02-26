using System;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.Net.Sockets;

namespace UAVCommons.Commands
{
    public interface CommandHandler
    {
     void CommandRecieved(TcpClient client,BaseCommand Command);

    }
}
