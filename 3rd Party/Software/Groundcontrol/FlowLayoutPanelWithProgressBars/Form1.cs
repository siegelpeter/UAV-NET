using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace FlowLayoutPanelWithDragging
{
    public partial class Form1 : Form
    {
        //private Panel flowLayoutPanel1 = new Panel();
        private List<DraggableUserControl> _items = new List<DraggableUserControl>();

        public Form1()
        {
            InitializeComponent();

            this.flowLayoutPanel1.DragEnter += new DragEventHandler(flowLayoutPanel1_DragEnter);
            this.flowLayoutPanel1.DragDrop += new DragEventHandler(flowLayoutPanel1_DragDrop);
            this.flowLayoutPanel2.DragEnter += new DragEventHandler(flowLayoutPanel1_DragEnter);
            this.flowLayoutPanel2.DragDrop += new DragEventHandler(flowLayoutPanel1_DragDrop);
        }

        void flowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            DraggableUserControl data = (DraggableUserControl)e.Data.GetData(typeof(DraggableUserControl));
            FlowLayoutPanel _destination = (FlowLayoutPanel)sender;
            FlowLayoutPanel _source = (FlowLayoutPanel)data.Parent;

            if (_source != _destination)
            {
                // Add control to panel
                _destination.Controls.Add(data);
                data.Size = new Size(_destination.Width, 50);
                
                // Reorder
                Point p = _destination.PointToClient(new Point(e.X, e.Y));
                var item = _destination.GetChildAtPoint(p);
                int index = _destination.Controls.GetChildIndex(item, false);
                _destination.Controls.SetChildIndex(data, index);

                // Invalidate to paint!
                _destination.Invalidate();
                _source.Invalidate();
            }
            else
            {
                // Just add the control to the new panel.
                // No need to remove from the other panel, this changes the Control.Parent property.
                Point p = _destination.PointToClient(new Point(e.X, e.Y));
                var item = _destination.GetChildAtPoint(p);
                int index = _destination.Controls.GetChildIndex(item, false);
                _destination.Controls.SetChildIndex(data, index);
                _destination.Invalidate();
            }
        }

        void flowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            Size s = new Size(flowLayoutPanel1.Width, 50);
            DraggableUserControl pgb;

            pgb = new DraggableUserControl();
            pgb.Padding = new Padding(5);
         
        
            pgb.Size = s;
            pgb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._items.Add(pgb);
            this.flowLayoutPanel1.Controls.Add(pgb);

            pgb = new DraggableUserControl();
            pgb.Padding = new Padding(5);
         
            pgb.Size = s;
            this._items.Add(pgb);
            this.flowLayoutPanel1.Controls.Add(pgb);

            pgb = new DraggableUserControl();
            pgb.Padding = new Padding(5);
        
            pgb.Size = s;
            this._items.Add(pgb);
            this.flowLayoutPanel1.Controls.Add(pgb);

            pgb = new DraggableUserControl();
            pgb.Padding = new Padding(5);
        
         
            pgb.Size = s;
            this._items.Add(pgb);
            this.flowLayoutPanel1.Controls.Add(pgb);

            pgb = new DraggableUserControl();
            pgb.Padding = new Padding(5);
           
            pgb.Size = s;
            this._items.Add(pgb);
            this.flowLayoutPanel1.Controls.Add(pgb);

        }
    }
}
