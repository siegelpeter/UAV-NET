using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GroundControlUI
{
    /// <summary>
    /// Display Position Waypoint path and Track via Google Earth
    /// http://code.google.com/intl/de-DE/apis/earth/documentation/geometries.html#linestring
    /// 
    /// </summary>
    public partial class GeoView : UserControl, PersistentData
    {
        public List<UAVCommons.Navigation.WayPoint> WayPoints = new List<UAVCommons.Navigation.WayPoint>();
        public GroundControlCore.GroundControlCore core = null;
        public bool centerOnUAV = false;
        
        public GeoView()
        {
        }

        public GeoView(GroundControlCore.GroundControlCore core)
        {
            this.core = core;
            core.currentUAV.DataArrived += new UAVCommons.UAVBase.DataArrivedHandler(MapView_ValueChanged);
                core.currentUAV.DataArrived += new UAVCommons.UAVBase.DataArrivedHandler(currentUAV_DataArrived);
           

            InitializeComponent();
            
        }

    


        public virtual string PersistentData
        {
            get
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream,WayPoints);
                
                return Convert.ToString(this.centerOnUAV) + ","+Convert.ToBase64String(stream.ToArray());
            }
            set
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                centerOnUAV = Convert.ToBoolean(value.Split(',')[0]);
                byte[] data = Convert.FromBase64String(value.Split(',')[1]);
                stream.Write(data, 0, data.Length);
                this.WayPoints = (List<UAVCommons.Navigation.WayPoint>) formatter.Deserialize(stream);
                
             
            }
        }

        public virtual void currentUAV_DataArrived(UAVCommons.CommunicationEndpoint source, UAVCommons.UAVSingleParameter arg)
        {

        }

        /// <summary>
        /// UAV Position hat sich Verändert aktualisiere Markierung auf der Karte
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        public virtual void MapView_ValueChanged(UAVCommons.CommunicationEndpoint source, UAVCommons.UAVSingleParameter arg)
        {

        }

        public void UpdateUAVPosition()
        {
        }

          }
}
