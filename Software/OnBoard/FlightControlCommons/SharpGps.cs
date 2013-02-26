
// Copyright 2007 - Morten Nielsen
//
// This file is part of SharpGps.
// SharpGps is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpGps is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpGps; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using SharpGis.SharpGps;
using SharpGis.SharpGps.NMEA;
using SharpMap;
using UAVCommons;
namespace FlightControlCommons
{
    [Serializable]
    public class GpsDevice {
    public static GPSHandler GPS;
        public string serialPort;
        public int BaudRate;
        public MonitoredDictionary<string, UAVSingleParameter> values = null;
		private int urate = 1000; // Updaterate for all Params
        public delegate void GPSvalueHandler(Dictionary<string, string> values);
        public event GPSvalueHandler ValuesChanged;

        public GpsDevice(HierachyItem owner)
        {
            GPS = new GPSHandler(); //Initialize GPS handler
            GPS.TimeOut = 5; //Set timeout to 5 seconds
            GPS.NewGPSFix += new GPSHandler.NewGPSFixHandler(this.GPSEventHandler); //Hook up GPS data events to a handle

            values = new MonitoredDictionary<string, UAVSingleParameter>(owner);
            if (!values.ContainsKey("lbRMCPosition")) values.Add(new UAVParameter("lbRMCPosition", GPS.GPRMC.Position, null,null, urate));
            if (!values.ContainsKey("lbRMCPositionLongitude")) values.Add(new UAVParameter("lbRMCPositionLongitude", 0, null,null, urate));
            if (!values.ContainsKey("lbRMCPositionLatitude")) values.Add(new UAVParameter("lbRMCPositionLatitude", 0, null,null, urate));
            if (!values.ContainsKey("lbRMCPositionUTM")) values.Add(new UAVParameter("lbRMCPositionUTM", 0, null,null, urate));
            if (!values.ContainsKey("lbRMCCourse")) values.Add(new UAVParameter("lbRMCCourse", 0, null,null, urate));
            if (!values.ContainsKey("lbRMCSpeed")) values.Add(new UAVParameter("lbRMCSpeed", 0, null,null, urate));
            if (!values.ContainsKey("lbRMCTimeOfFix")) values.Add(new UAVParameter("lbRMCTimeOfFix", 0, null,null, urate));
            if (!values.ContainsKey("lbRMCMagneticVariation")) values.Add(new UAVParameter("lbRMCMagneticVariation", 0, null,null, urate));
            if (!values.ContainsKey("lbGGAPosition")) values.Add(new UAVParameter("lbGGAPosition", 0, null,null, urate));
            if (!values.ContainsKey("lbGGATimeOfFix")) values.Add(new UAVParameter("lbGGATimeOfFix", 0, null,null, urate));
            if (!values.ContainsKey("lbGGAFixQuality")) values.Add(new UAVParameter("lbGGAFixQuality", 0, null,null, urate));
            if (!values.ContainsKey("lbGGANoOfSats")) values.Add(new UAVParameter("lbGGANoOfSats", 0, null,null, urate));
            if (!values.ContainsKey("lbGGAAltitude")) values.Add(new UAVParameter("lbGGAAltitude", 0, null,null, urate));
            if (!values.ContainsKey("lbGGAAltitudeUnit")) values.Add(new UAVParameter("lbGGAAltitudeUnit", 0, null,null, urate));

            if (!values.ContainsKey("lbGGAHDOP")) values.Add(new UAVParameter("lbGGAHDOP", 0, null,null, urate));
            if (!values.ContainsKey("lbGGAGeoidHeight")) values.Add(new UAVParameter("lbGGAGeoidHeight", 0, null,null, urate));
            if (!values.ContainsKey("lbGGADGPSupdate")) values.Add(new UAVParameter("lbGGADGPSupdate", 0, null,null, urate));
            if (!values.ContainsKey("lbGGADGPSID")) values.Add(new UAVParameter("lbGGADGPSID", 0, null,null, urate));
            if (!values.ContainsKey("lbGLLPosition")) values.Add(new UAVParameter("lbGLLPosition", 0, null,null, urate));
            if (!values.ContainsKey("lbGLLTimeOfSolution")) values.Add(new UAVParameter("lbGLLTimeOfSolution", 0, null,null, urate));
            if (!values.ContainsKey("lbGLLDataValid")) values.Add(new UAVParameter("lbGLLDataValid", 0, null,null, urate));
            if (!values.ContainsKey("lbGSAMode")) values.Add(new UAVParameter("lbGSAMode", 0, null,null, urate));
            if (!values.ContainsKey("lbGLLPosition")) values.Add(new UAVParameter("lbGLLPosition", 0,null,null, urate));
            if (!values.ContainsKey("lbGSAFixMode")) values.Add(new UAVParameter("lbGSAFixMode", 0, null,null, urate));
            if (!values.ContainsKey("lbGSAPRNs")) values.Add(new UAVParameter("lbGSAPRNs", 0, null,null, urate));
            if (!values.ContainsKey("lbGSAPDOP")) values.Add(new UAVParameter("lbGSAPDOP", 0, null,null, urate));
            if (!values.ContainsKey("lbGSAHDOP")) values.Add(new UAVParameter("lbGSAHDOP", 0, null,null, urate));
            if (!values.ContainsKey("lbGSAVDOP")) values.Add(new UAVParameter("lbGSAVDOP", 0, null,null, urate));
            if (!values.ContainsKey("lbRMEHorError")) values.Add(new UAVParameter("lbRMEHorError", 0, null,null, urate));
            if (!values.ContainsKey("lbRMEVerError")) values.Add(new UAVParameter("lbRMEVerError", 0, null,null, urate));
            if (!values.ContainsKey("lbRMESphericalError")) values.Add(new UAVParameter("lbRMESphericalError", 0, null,null, urate));
            if (!values.ContainsKey("lbRMEVerError")) values.Add(new UAVParameter("lbRMEVerError", 0, null,null, urate));
            if (!values.ContainsKey("lbRMEVerError")) values.Add(new UAVParameter("lbRMEVerError", 0, null,null, urate));
	
        }

        public void initgps(string serialport,int baud)
		{
            this.serialPort = serialport;
            this.BaudRate = baud;
            values.AddOnSet = true;
            
    

		 start();
        }

		public void start()
		{
			
			
		
					GPS.Start(this.serialPort, BaudRate); //Open serial port

		}

        private double[] TransformToUTM(SharpGis.SharpGps.Coordinate p)
        {
            //For fun, let's use the SharpMap transformation library and display the position in UTM
            int zone = (int)Math.Floor((p.Longitude + 183) / 6.0);
            SharpMap.CoordinateSystems.ProjectedCoordinateSystem proj = SharpMap.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, (p.Latitude >= 0));
            SharpMap.CoordinateSystems.Transformations.ICoordinateTransformation trans =
                new SharpMap.CoordinateSystems.Transformations.CoordinateTransformationFactory().CreateFromCoordinateSystems(proj.GeographicCoordinateSystem, proj);
            double[] result = trans.MathTransform.Transform(new double[] { p.Longitude, p.Latitude });
            return new double[] { result[0], result[1], zone };
        }
		/// <summary>
		/// Responds to sentence events from GPS receiver
		/// </summary>
		private void GPSEventHandler(object source, GPSEventArgs e)
		{
            
          	switch (e.TypeOfEvent)
			{
				case GPSEventType.GPRMC:  //Recommended minimum specific GPS/Transit data
					if (GPS.HasGPSFix) //Is a GPS fix available?
					{
						//lbRMCPosition.Text = GPS.GPRMC.Position.ToString("#.000000");
						values["lbRMCPosition"].Value=GPS.GPRMC.Position.ToString("DMS");
                        values["lbRMCPositionLongitude"].Value=GPS.GPRMC.Position.Longitude;
                        values["lbRMCPositionLatitude"].Value= GPS.GPRMC.Position.Latitude;
						
                        double[] utmpos = TransformToUTM(GPS.GPRMC.Position);
						values["lbRMCPositionUTM"].Value=utmpos[0].ToString("#.0N ") + utmpos[0].ToString("#.0E") + " (Zone: " + utmpos[2] + ")";
						values["lbRMCCourse"].Value=GPS.GPRMC.Course.ToString();
						values["lbRMCSpeed"].Value=GPS.GPRMC.Speed.ToString() + " mph";
						values["lbRMCTimeOfFix"].Value=GPS.GPRMC.TimeOfFix.ToString("F");
						values["lbRMCMagneticVariation"].Value=GPS.GPRMC.MagneticVariation.ToString();
					}
					else
					{
						values["lbRMCCourse"].Value="N/A";
					values["lbRMCSpeed"].Value = "N/A";
						values["lbRMCTimeOfFix"].Value = GPS.GPRMC.TimeOfFix.ToString();
					}
					break;
				case GPSEventType.GPGGA: //Global Positioning System Fix Data
					if(GPS.GPGGA.Position!=null)
					values["lbGGAPosition"].Value = GPS.GPGGA.Position.ToString("DM");
					else
					values["lbGGAPosition"].Value="";
					values["lbGGATimeOfFix"].Value=GPS.GPGGA.TimeOfFix.Hour.ToString() + ":" + GPS.GPGGA.TimeOfFix.Minute.ToString() + ":" + GPS.GPGGA.TimeOfFix.Second.ToString();
					values["lbGGAFixQuality"].Value=GPS.GPGGA.FixQuality.ToString();
					values["lbGGANoOfSats"].Value=GPS.GPGGA.NoOfSats.ToString();
					values["lbGGAAltitude"].Value=GPS.GPGGA.Altitude.ToString();
                    values["lbGGAAltitudeUnit"].Value=GPS.GPGGA.AltitudeUnits;
					
                    values["lbGGAHDOP"].Value=GPS.GPGGA.Dilution.ToString();
					values["lbGGAGeoidHeight"].Value=GPS.GPGGA.HeightOfGeoid.ToString();
					values["lbGGADGPSupdate"].Value=GPS.GPGGA.DGPSUpdate.ToString();
					values["lbGGADGPSID"].Value=GPS.GPGGA.DGPSStationID;
					break;
				case GPSEventType.GPGLL: //Geographic position, Latitude and Longitude
					values["lbGLLPosition"].Value=GPS.GPGLL.Position.ToString();
				values["lbGLLTimeOfSolution"].Value=(GPS.GPGLL.TimeOfSolution.HasValue ? GPS.GPGLL.TimeOfSolution.Value.Hours.ToString() + ":" + GPS.GPGLL.TimeOfSolution.Value.Minutes.ToString() + ":" + GPS.GPGLL.TimeOfSolution.Value.Seconds.ToString() : "");
					values["lbGLLDataValid"].Value=GPS.GPGLL.DataValid.ToString();
					break;
				case GPSEventType.GPGSA: //GPS DOP and active satellites
					if (GPS.GPGSA.Mode == 'A')
						values["lbGSAMode"].Value="Auto";
					else if (GPS.GPGSA.Mode == 'M')
						values["lbGSAMode"].Value="Manual";
					else values["lbGSAMode"].Value="";
					values["lbGSAFixMode"].Value=GPS.GPGSA.FixMode.ToString();
					values["lbGSAPRNs"].Value="";
					if(GPS.GPGSA.PRNInSolution.Count>0)
						foreach (string prn in GPS.GPGSA.PRNInSolution)
							values["lbGSAPRNs"].Value+= prn + " ";
					else
						values["lbGSAPRNs"].Value += "none";
					values["lbGSAPDOP"].Value=GPS.GPGSA.PDOP.ToString() + " (" + DOPtoWord(GPS.GPGSA.PDOP) +")";
					values["lbGSAHDOP"].Value=GPS.GPGSA.HDOP.ToString() + " (" + DOPtoWord(GPS.GPGSA.HDOP) + ")";
					values["lbGSAVDOP"].Value=GPS.GPGSA.VDOP.ToString() + " (" + DOPtoWord(GPS.GPGSA.VDOP) + ")";
					break;
				case GPSEventType.GPGSV: //Satellites in view
					//if (NMEAtabs.TabPages[NMEAtabs.SelectedIndex].Text == "GPGSV") //Only update this tab when it is active
					//	DrawGSV();
					break;
				case GPSEventType.PGRME: //Garmin proprietary sentences.
					values["lbRMEHorError"].Value=GPS.PGRME.EstHorisontalError.ToString();
					values["lbRMEVerError"].Value=GPS.PGRME.EstVerticalError.ToString();
					values["lbRMESphericalError"].Value=GPS.PGRME.EstSphericalError.ToString();
					break;
				case GPSEventType.TimeOut: //Serialport timeout.
					/*notification1.Caption = "GPS Serialport timeout";
					notification1.InitialDuration = 5;
					notification1.Text = "Check your settings and connection";
					notification1.Critical = false;
					notification1.Visible = true;
					 */
					break;
			}
      
            /// </param>   if (ValuesChanged != null) ValuesChanged(values);///
           }
		private string DOPtoWord(double dop)
		{
			if (dop < 1.5) return "Ideal";
			else if (dop < 3) return "Excellent";
			else if (dop < 6) return "Good";
			else if (dop < 8) return "Moderate";
			else if (dop < 20) return "Fair";
			else return "Poor";
		}
	/*	private void DrawGSV()
		{
			System.Drawing.Color[] Colors = { Color.Blue , Color.Red , Color.Green, Color.Yellow, Color.Cyan, Color.Orange,
											  Color.Gold , Color.Violet, Color.YellowGreen, Color.Brown, Color.GreenYellow,
											  Color.Blue , Color.Red , Color.Green, Color.Yellow, Color.Aqua, Color.Orange};
			//Generate signal level readout
			int SatCount = GPS.GPGSV.SatsInView;
			Bitmap imgSignals = new Bitmap(picGSVSignals.Width, picGSVSignals.Height);
			Graphics g = Graphics.FromImage(imgSignals);
			g.Clear(Color.White);
			Pen penBlack = new Pen(Color.Black, 1);
			Pen penBlackDashed = new Pen(Color.Black, 1);
			penBlackDashed.DashPattern = new float[] { 2f , 2f };
			Pen penGray = new Pen(Color.LightGray, 1);
			int iMargin = 4; //Distance to edge of image
			int iPadding = 4; //Distance between signal bars
			g.DrawRectangle(penBlack, 0, 0, imgSignals.Width - 1, imgSignals.Height - 1);
			
			StringFormat sFormat =	new StringFormat();
			int barWidth = 1;
			if(SatCount>0)
				barWidth = (imgSignals.Width - 2 * iMargin-iPadding*(SatCount-1)) / SatCount;

			//Draw horisontal lines
			for (int i = imgSignals.Height - 15; i > iMargin; i -= (imgSignals.Height - 15 - iMargin) / 5)
				g.DrawLine(penGray, 1, i, imgSignals.Width - 2, i);
			sFormat.Alignment = StringAlignment.Center;
			//Draw satellites
			//GPS.GPGSV.Satellites.Sort(); //new Comparison<SharpGis.SharpGps.NMEA.GPGSV.Satellite>(sat1, sat2) { int.Parse(sat1)
			for (int i = 0; i < GPS.GPGSV.Satellites.Count; i++)
			{
				SharpGis.SharpGps.NMEA.GPGSV.Satellite sat = GPS.GPGSV.Satellites[i];
				int startx = i * (barWidth + iPadding)+iMargin;
				int starty = imgSignals.Height - 15;
				int height = (imgSignals.Height - 15 - iMargin)/50*sat.SNR;
				if (GPS.GPGSA.PRNInSolution.Contains(sat.PRN))
				{
					g.FillRectangle(new System.Drawing.SolidBrush(Colors[i]), startx, starty - height + 1, barWidth, height);
					g.DrawRectangle(penBlack, startx, starty - height, barWidth, height);}
				else
				{
					g.FillRectangle(new System.Drawing.SolidBrush(Color.FromArgb(50, Colors[i])), startx, starty - height + 1, barWidth, height);
					g.DrawRectangle(penBlackDashed, startx, starty - height, barWidth, height);
				}
								
				sFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(sat.PRN,new Font("Verdana",9,FontStyle.Regular),new System.Drawing.SolidBrush(Color.Black),startx+barWidth/2,imgSignals.Height-15,sFormat);
				sFormat.LineAlignment = StringAlignment.Far;
				g.DrawString(sat.SNR.ToString(), new Font("Verdana", 9, FontStyle.Regular), new System.Drawing.SolidBrush(Color.Black), startx + barWidth / 2, starty-height, sFormat);
			}
			picGSVSignals.Image = imgSignals;

			//Generate sky view
			Bitmap imgSkyview = new Bitmap(picGSVSkyview.Width, picGSVSkyview.Height);
			g = Graphics.FromImage(imgSkyview);
			g.Clear(Color.Transparent);
			g.FillEllipse(Brushes.White, 0, 0, imgSkyview.Width - 1, imgSkyview.Height - 1);
			g.DrawEllipse(penGray, 0, 0, imgSkyview.Width - 1, imgSkyview.Height - 1);
			g.DrawEllipse(penGray, imgSkyview.Width / 4, imgSkyview.Height / 4, imgSkyview.Width / 2, imgSkyview.Height / 2);
			g.DrawLine(penGray, imgSkyview.Width / 2, 0, imgSkyview.Width / 2, imgSkyview.Height);
			g.DrawLine(penGray, 0, imgSkyview.Height / 2, imgSkyview.Width, imgSkyview.Height / 2);
			sFormat.LineAlignment = StringAlignment.Near;
			sFormat.Alignment = StringAlignment.Near;
			float radius = 6f;
			for (int i = 0; i < GPS.GPGSV.Satellites.Count; i++)
			{
				SharpGis.SharpGps.NMEA.GPGSV.Satellite sat = GPS.GPGSV.Satellites[i];
				double ang = 90.0-sat.Azimuth;
				ang = ang/180.0*Math.PI;
				int x = imgSkyview.Width/2 + (int)Math.Round((Math.Cos(ang) * ((90.0 - sat.Elevation)/90.0)*(imgSkyview.Width/2.0-iMargin)));
				int y = imgSkyview.Height/2 - (int)Math.Round((Math.Sin(ang) * ((90.0 - sat.Elevation) / 90.0) * (imgSkyview.Height / 2.0-iMargin)));
				g.FillEllipse(new System.Drawing.SolidBrush(Colors[i]), x - radius * 0.5f, y - radius * 0.5f, radius, radius);

				if (GPS.GPGSA.PRNInSolution.Contains(sat.PRN))
				{
					g.DrawEllipse(penBlack, x - radius * 0.5f, y - radius * 0.5f, radius, radius);
					g.DrawString(sat.PRN, new Font("Verdana", 9, FontStyle.Bold), new System.Drawing.SolidBrush(Color.Black), x, y, sFormat);
				}
				else
					g.DrawString(sat.PRN, new Font("Verdana", 8, FontStyle.Italic), new System.Drawing.SolidBrush(Color.Gray), x, y, sFormat);
			}
			picGSVSkyview.Image = imgSkyview;
		}

				
		private void btnNTRIPGetSourceTable_Click(object sender, EventArgs e)
		{
			SharpGis.SharpGps.NTRIP.NTRIPClient ntrip = new SharpGis.SharpGps.NTRIP.NTRIPClient(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(tbNTRIPServerIP.Text.Trim()), int.Parse(tbNTRIPPort.Text)));
			// http://igs.ifag.de/root_ftp/misc/ntrip/streamlist_euref-ip.htm
				
			SharpGis.SharpGps.NTRIP.SourceTable table = ntrip.GetSourceTable();
			if (table != null)
			{
				dgNTRIPCasters.DataSource = table.Casters;
				dgNTRIPNetworks.DataSource = table.Networks;
				if (table.DataStreams.Count > 0)
					//ntrip.StartNTRIP(table.DataStreams[0].MountPoint);
					ntrip.StartNTRIP("FFMJ2");
					
				else
					MessageBox.Show("Sourcetable doesn't contain any datastreams");
			}
			else
				MessageBox.Show("Failed to request or parse the DataSource Table");
		}
        */
	
	}
}
