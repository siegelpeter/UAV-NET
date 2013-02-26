using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Commands
{
    /// <summary>
    /// Schreibt einen Text in die Ausgabe der Bodenstation
    /// Dieses Kommando sollte nur vom UAV aus gesendet werden.
    /// </summary>
       [Serializable]
    public class WriteToFlightLog:BaseCommand
    {
        public string logLine = null;
        string sourceName = null;
        public DateTime timestamp = DateTime.MinValue;

        /// <summary>
        /// Constructor which sets the Payload e.g. Message and Timetamp of the Command
        /// use SendCommand or SendCommandAync to Transmit it to the other Station
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="timestamp"></param>
        public WriteToFlightLog(string Line, DateTime timestamp):base("WriteToFlightLog") {
            this.logLine = Line;
            this.timestamp = timestamp;

        }

        public override void Send(UAVBase core)
        {
            base.Send(core);
            sourceName = "UAV";
        }
        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
            core.WriteToLog(sourceName, "FlightLog", logLine);   
        }
    }
}
