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
using System.Globalization;

namespace GroundControlUI
{

    public partial class MapView : GeoView
    {
        GMap.NET.WindowsForms.GMapOverlay overlay = null;
        GMap.NET.WindowsForms.GMapOverlay UAVOverlay = null;
        Navigation.EditWayPoint frm = null;
        UAVCommons.UAVStructure GPS = null;
        GMapRoute route = null;
        GMapMarker currentmarker = null;
        GMapMarker lastmarker = null;
        int lastx = 0;
        int lasty = 0;
        bool centerOnUAV = false;
        
        public MapView()
        {
        }

        public MapView(GroundControlCore.GroundControlCore core):base(core)
        {
            InitializeComponent();
            gMapControl1.RoutesEnabled = true;
            overlay = new GMap.NET.WindowsForms.GMapOverlay(gMapControl1, "Waypoints");
            this.gMapControl1.Overlays.Add(overlay);
            UAVOverlay = new GMap.NET.WindowsForms.GMapOverlay(gMapControl1, "UAV");
            this.gMapControl1.Overlays.Add(UAVOverlay);
           
            gMapControl1.HoldInvalidation = false;
            gMapControl1.ZoomAndCenterRoutes(null);
            gMapControl1.ZoomAndCenterMarkers(null);
            gMapControl1.CanDragMap = true;
            gMapControl1.DragButton = System.Windows.Forms.MouseButtons.Left;
            gMapControl1.SetCurrentPositionByKeywords("Leonding");
            gMapControl1.Zoom = 17;
            overlay.Markers.CollectionChanged += new GMap.NET.ObjectModel.NotifyCollectionChangedEventHandler(Markers_CollectionChanged);

        }

    
        

        public string PersistentData
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

        void currentUAV_DataArrived(UAVCommons.CommunicationEndpoint source, UAVCommons.UAVParameter arg)
        {

        }

        /// <summary>
        /// UAV Position hat sich Verändert aktualisiere Markierung auf der Karte
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        public  void MapView_ValueChanged(UAVCommons.CommunicationEndpoint source, UAVCommons.UAVSingleParameter arg)
        {
        
            //if (arg.Name == "lbRMCPositionLatitude")
            //{
            //    if (gMapControl1.InvokeRequired)
            //    {

            //        gMapControl1.Invoke(new MethodInvoker(UpdateUAVPosition));
            //    }
            //    else
            //    {
            //        UpdateUAVPosition();

            //    }
            //}
        }

        private void UpdateUAVPosition()
        {
            if (GPS != null)
            {
                  CultureInfo cultureInfo = new CultureInfo("en-US");
                 
                lbl_hasfix.Text = "GPS State:" + GPS["lbGSAFixMode"].Value.ToString() + " Altitude " + GPS["lbGGAAltitude"].Value.ToString() + " " + GPS["lbGGAAltitudeUnit"].Value.ToString()+ " Speed "+GPS["lbRMCSpeed"].Value.ToString();
                GMap.NET.PointLatLng point = new GMap.NET.PointLatLng(Convert.ToDouble(GPS["lbRMCPositionLatitude"].Value, cultureInfo), Convert.ToDouble(GPS["lbRMCPositionLongitude"].Value, cultureInfo));
                if (UAVOverlay != null)
                {
                    if (UAVOverlay.Markers.Count == 0)
                    {
                        UAVOverlay.Markers.Add(new GMapMarkerGoogleGreen(point));
                    }
                    else
                    {
                        UAVOverlay.Markers[0].Position = point;
                    }
                    if (centerOnUAV)
                    {
                        this.gMapControl1.Position = point;
                        this.gMapControl1.Update();
                    }
                    this.gMapControl1.UpdateMarkerLocalPosition(UAVOverlay.Markers[0]);
                }
            }
        }

        void Markers_CollectionChanged(object sender, GMap.NET.ObjectModel.NotifyCollectionChangedEventArgs e)
        {

            int i = 0;
            foreach (GMapMarker marker in overlay.Markers)
            {
                marker.Tag = i;
                string altstring = " (";

                if (WayPoints[i].IsAbsolute)
                {
                    altstring += WayPoints[i].Altitude + " M";
                }
                else {
                    altstring += WayPoints[i].AltitudeAGL + " M AGL";
                }
                altstring += ")";

                marker.ToolTipText = "Wegpunkt " + i+ altstring;
                i++;

            }
        }

        private void gMapControl1_OnMarkerClick(GMap.NET.WindowsForms.GMapMarker item, MouseEventArgs e)
        {
            currentmarker = item;

        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.contextMenuStrip1.Close();
                lastx = e.X;
                lasty = e.Y;
                if (currentmarker != null)
                {
                    this.wegpunktLöschenToolStripMenuItem.Visible = true;
                    this.neuerWegpunktToolStripMenuItem.Visible = false;
                    this.wegpunktBearbeitenToolStripMenuItem.Visible = true;

                }
                else
                {
                    this.neuerWegpunktToolStripMenuItem.Visible = true;
                    this.wegpunktLöschenToolStripMenuItem.Visible = false;
                    this.wegpunktBearbeitenToolStripMenuItem.Visible = false;
                }
                this.contextMenuStrip1.Show(this, e.X, e.Y);


            }
        }

        /// <summary>
        /// Reload Markers and route Overlay
        /// </summary>
        public void Refreshwpt()
        {
            /*overlay.Markers.Clear();
            foreach (UAVCommons.Navigation.WayPoint wpt in WayPoints){
                GMap.NET.WindowsForms.GMapMarker marker = new GMap.NET.WindowsForms.GMapMarker(new GMap.NET.PointLatLng(wpt.Latitude, wpt.Longitude))
                marker.IsVisible = true;
                marker.Tag = wpt;
                overlay.Markers.Add(marker);
            }*/
            List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>();

            foreach (UAVCommons.Navigation.WayPoint wpt in WayPoints)
            {
                CreateWayPoint(new GMap.NET.PointLatLng(wpt.Latitude, wpt.Longitude));

            }
        }

        private void neuerWegpunktToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (route == null)
            {
                overlay.Routes.Clear();

                List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>();
                route = new GMapRoute(points, "Aktuelle Route");
                overlay.Routes.Add(route);

            }

            GMap.NET.PointLatLng point = this.gMapControl1.FromLocalToLatLng(lastx, lasty);
            point = CreateWayPoint(point);
            gMapControl1.Invalidate(false);
            gMapControl1.Refresh();

        }

        private GMap.NET.PointLatLng CreateWayPoint(GMap.NET.PointLatLng point)
        {
            frm = new Navigation.EditWayPoint(1200, true);
            if (frm.Altitude == 0)
            {
                if (frm.ShowDialog() == DialogResult.Abort) return new GMap.NET.PointLatLng();
            }
            route.Points.Add(point);
            UAVCommons.Navigation.WayPoint wpoint = new UAVCommons.Navigation.WayPoint();

            wpoint.Longitude = point.Lng;
            wpoint.Latitude = point.Lat;
            string altstring = " (";
            altstring += frm.Altitude;
            wpoint.IsAbsolute = frm.IsAbsolute;
            
            if (frm.IsAbsolute)
            {
                wpoint.Altitude = frm.Altitude;
                altstring += "M";
                wpoint.AltitudeAGL = 0;
            }
            else
            {
                wpoint.AltitudeAGL = frm.Altitude;
                wpoint.Altitude = 0;
                altstring+= "M AGL";
            }
           
            altstring += ")";
            WayPoints.Add(wpoint);
            
            GMapMarker mymarker = new GMapMarkerGoogleRed(point);
            mymarker.Tag = route.Points.Count - 1;
            
            mymarker.ToolTipMode = MarkerTooltipMode.Always;
            mymarker.ToolTipText = "Wegpunkt " + Convert.ToString(route.Points.Count) + altstring;
            overlay.Markers.Add(mymarker);

            route.Overlay.IsVisibile = true;
            route.IsVisible = true;
            gMapControl1.UpdateRouteLocalPosition(route);
            gMapControl1.UpdateMarkerLocalPosition(mymarker);

            return point;
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (currentmarker != null)
                {
                    if (currentmarker.IsVisible)
                    {
                        currentmarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                        if (currentmarker.Tag != null)
                        {
                            int index = (int)currentmarker.Tag;
                            route.Points[index] = currentmarker.Position;
                            this.WayPoints[index].Latitude = currentmarker.Position.Lat;
                            this.WayPoints[index].Longitude = currentmarker.Position.Lng;

                            gMapControl1.UpdateMarkerLocalPosition(currentmarker);
                            gMapControl1.UpdateRouteLocalPosition(route);
                        }
                    }
                }
            }

        }

        private void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            currentmarker = item;
            lastmarker = item;
        }

        /// <summary>
        /// Lösche aktuellen Wegpunkt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wegpunktLöschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastmarker != null)
            {
                if (lastmarker.IsVisible)
                {
                    int index = (int)lastmarker.Tag;
                    route.Points.RemoveAt(index);
                    this.WayPoints.RemoveAt(index);
                    lastmarker.IsVisible = false;
                    gMapControl1.UpdateMarkerLocalPosition(lastmarker);
                    lastmarker.Overlay.Markers.Remove(lastmarker);
                    lastmarker = null;
                    gMapControl1.UpdateRouteLocalPosition(route);
                }
            }

        }

        private void gMapControl1_OnMarkerLeave(GMapMarker item)
        {

        }

        private void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            currentmarker = null;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            core.currentUAV.SendCommand(new FlightControlCommons.components.UpdateWayPoints(WayPoints,"MissionControl",0));
        }

        private void wegpunktBearbeitenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = ((int)this.lastmarker.Tag);
            
            Navigation.EditWayPoint frm = new Navigation.EditWayPoint(666,WayPoints[index].IsAbsolute);;
            
           // frm.IsAbsolute = WayPoints[index].IsAbsolute;
            if (frm.IsAbsolute)
            {
                frm.Altitude = Convert.ToInt32(WayPoints[index].Altitude);
            }
            else
            {
                frm.Altitude = Convert.ToInt32(WayPoints[index].AltitudeAGL);
       
            }

            if (frm.ShowDialog() == DialogResult.OK) { 
                if (frm.IsAbsolute){
                    WayPoints[index].Altitude = frm.Altitude;
                }else{
                    WayPoints[index].AltitudeAGL = frm.Altitude;
                }
                WayPoints[index].IsAbsolute = frm.IsAbsolute;
                this.gMapControl1.UpdateMarkerLocalPosition(lastmarker);
                Markers_CollectionChanged(null, null);
            }
        }

        private void btn_centeronuav_Click(object sender, EventArgs e)
        {
            centerOnUAV = true;
        }

        private void btn_free_Click(object sender, EventArgs e)
        {
            centerOnUAV = false;
        }

        private void SearchGPS_Click(object sender, EventArgs e)
        {
           
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (GPS == null) {
                try
                {
                    foreach (var item in core.currentUAV.uavData)
                    {
                        if ((item.Value is UAVCommons.UAVStructure) && (((UAVCommons.UAVStructure)item.Value).values.ContainsKey("lbRMCPositionLatitude")))
                        {
                            this.GPS = (UAVCommons.UAVStructure)item.Value;
                            return;
                        }

                    }
                }
                catch (Exception ex) { 
                }
            }
            UpdateUAVPosition();
        }
    }
}
