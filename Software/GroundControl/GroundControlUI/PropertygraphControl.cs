using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using UAVCommons;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
namespace GroundControlUI
{

    public partial class PropertygraphControl : UserControl, PersistentData
    {
        
       // PointPairList list = null;
        GraphPane myPane = null;  // The GraphObject to draw on
        List<string> selectedKeys = new List<string>(); // Selected Parameters to display
        GroundControlCore.GroundControlCore core = null;
        /// <summary>
        /// Initialises the Graphcontrol 
        /// 
        /// </summary>
        /// <param name="core"></param>
        public PropertygraphControl(GroundControlCore.GroundControlCore core)
        {
            InitializeComponent();
            this.comboBox1.Items.Clear();

            List<UAVSingleParameter> uavData = MonitoredDictionary<string, UAVSingleParameter>.NormaliseDictionary(core.currentUAV.uavData);

            foreach (UAVSingleParameter param in uavData)
            {
                comboBox1.Items.Add(param.GetStringPath());
            }

            this.core = core;
           myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Live Data";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Values";
            myPane.XAxis.Type = AxisType.Date;
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.Scale.MaxAuto = true;
            
            zedGraphControl1.IsAutoScrollRange = true;
            zedGraphControl1.IsEnableWheelZoom = true;
            zedGraphControl1.IsEnableHPan = true;
            myPane.Legend.IsVisible = true;
        }


        public string PersistentData
        {
            get
            {
                return String.Join(",",selectedKeys);
            }
            set
            {
                this.selectedKeys.Clear();
                this.selectedKeys.AddRange(value.Split(','));

            }
        }


  

        public void zoom(){
            //Set the displayed range (Max - 5 time units)
            zedGraphControl1.GraphPane.XAxis.Scale.Min = (double)new XDate(DateTime.Now.AddSeconds(-30));
            zedGraphControl1.GraphPane.XAxis.Scale.Max = (double)new XDate(DateTime.Now);

            //Set the max scrollable area to be all the data
            zedGraphControl1.ScrollMinX = (double)new XDate(DateTime.Now.AddHours(-1));
            zedGraphControl1.ScrollMaxX = (double)new XDate(DateTime.Now);

            //redraw
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
            
        }
      

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Shows Chooser Form and updates the Graph afterwards
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            PropertyChooser myfrm = new PropertyChooser(core,this.selectedKeys);
            myfrm.ShowDialog();
            this.selectedKeys = myfrm.choosenKeys;
            foreach (string curvename in this.selectedKeys) {
                CreateNewCurve(curvename);
            }
        }

        /// <summary>
        /// Quickly Sets the Graph to one Choosen Curve only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex > 0) {
                myPane.CurveList.Clear();
                CreateNewCurve(comboBox1.SelectedItem.ToString());
            }
        }

        /// <summary>
        /// Creates a New Cureve in the Graph with a given Name
        /// </summary>
        /// <param name="name"></param>
        private void CreateNewCurve(string name)
        {
            if (!HasCurve(name))
            {
                // Make up some data points from the Sine function
                PointPairList list = new PointPairList();

                // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
                LineItem myCurve = myPane.AddCurve(name, list, Color.Blue,
                                  SymbolType.Circle);
                
                // Fill the area under the curve with a white-red gradient at 45 degrees
                myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
                // Make the symbols opaque by filling them with white
                myCurve.Symbol.Fill = new Fill(Color.White);

                // Fill the axis background with a color gradient
                myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);

                // Fill the pane background with a color gradient
                myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
                
                // Calculate the Axis Scale Ranges
                this.zedGraphControl1.AxisChange();
            }
        }

        /// <summary>
        /// Checks if a Curve with a given Name has been Created in the Graph
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool HasCurve(string name)
        {
             foreach (CurveItem myitem in myPane.CurveList) {
                if (myitem.Label.Text == name) {
                    return true;
                }
            }
            return false;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            foreach (var param in this.core.currentUAV.uavData.Values){
            foreach (LineItem myitem in myPane.CurveList) // For all Curves in the graph
            {
                if (myitem.Label.Text == param.GetStringPath()) // if we've found the correct curve
                {
                    // Add the new Value
                    myitem.AddPoint(new PointPair(((double)new XDate(DateTime.Now)), Convert.ToDouble(param.Value)));
                    if (myitem.Points.Count > 50) myitem.RemovePoint(0); // Remove one old Value
                    if ((param.Min != null) && (myPane.YAxis.Scale.Min > Convert.ToInt32(param.Min)))
                    {
                        myPane.YAxis.Scale.Min = Convert.ToInt32(param.Min);
                    }
                    if ((param.Max != null) && (myPane.YAxis.Scale.Max < Convert.ToInt32(param.Max)))
                    {
                        myPane.YAxis.Scale.Max = Convert.ToInt32(param.Max);
                    }
                }
            }
        
           
        }
            zoom();

        }

    }
}
