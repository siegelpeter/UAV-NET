using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using System.Drawing;

using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.ComponentModel;

namespace FlowLayoutPanelWithDragging
{
    public class DraggableUserControl : UserControl
    {
        
        //Check radius for begin drag n drop
        public bool AllowDrag { get; set; }
        private bool _isDragging = false;
        private int _DDradius = 40;
        private int _mX = 0;
        private int _mY=0;

        public DraggableUserControl()
        {
            //Font = new Font("Arial", 10);
            this.InitializeComponent();
            Margin = new Padding(0);
            AllowDrag = true;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
   
        }

     

        protected override void OnGotFocus(EventArgs e)
        {
            this.BackColor = SystemColors.ActiveCaption;
                //Color.SandyBrown;
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            this.BackColor = this.FindForm().ContainsFocus ? Color.Transparent : SystemColors.InactiveCaption;
            base.OnLostFocus(e);
        }

        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            base.OnClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
            _mX = e.X;
            _mY = e.Y;
            this._isDragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDragging)
            {
                // This is a check to see if the mouse is moving while pressed.
                // Without this, the DragDrop is fired directly when the control is clicked, now you have to drag a few pixels first.
                if (e.Button == MouseButtons.Left && _DDradius > 0 && this.AllowDrag)
                {
                    int num1 = _mX - e.X;
                    int num2 = _mY - e.Y;
                    if (((num1 * num1) + (num2 * num2)) > _DDradius)
                    {
                        DoDragDrop(this, DragDropEffects.All);
                        _isDragging = true;
                        return;
                    }
                }
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
            base.OnMouseUp(e);
        }

        protected override void OnCreateControl()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.Selectable | ControlStyles.CacheText
                , true);


            this.DoubleBuffered = true;
            base.OnCreateControl();
        
        }

        protected override void OnPaint(PaintEventArgs e)
        {
          /*  Graphics g = e.Graphics;

            using (LinearGradientBrush _LeftAndRightBrush = new LinearGradientBrush(mainArea, Color.DimGray, Color.Black, LinearGradientMode.Vertical)
                , _StatusBrush = new LinearGradientBrush(mainArea, StatusColor1, StatusColor2, LinearGradientMode.Vertical)
                , _MainBrush = new LinearGradientBrush(mainArea, FirstColor, SecondColor, LinearGradientMode.Vertical))
            {
                using (StringFormat f = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    // Draw left
                    if (LeftBarSize > 0)
                    {
                        g.FillRoundedRectangle(_LeftAndRightBrush, leftArea, this.RoundedCornerAngle, RectangleEdgeFilter.TopLeft | RectangleEdgeFilter.BottomLeft);
                        g.DrawString(this.LeftText, this.Font, Brushes.White, leftArea, f);
                    }

                    // Draw status
                    if (StatusBarSize > 0)
                    {
                        g.FillRoundedRectangle(_StatusBrush, statusArea, this.RoundedCornerAngle, RectangleEdgeFilter.None);
                        g.DrawString(this.StatusText, this.Font, Brushes.White, statusArea, f);
                    }

                    // Draw main background
                    g.FillRoundedRectangle(Brushes.DimGray, mainAreaBackground, this.RoundedCornerAngle, RectangleEdgeFilter.None);

                    // Draw main
                    g.FillRoundedRectangle(_MainBrush, mainArea, this.RoundedCornerAngle, RectangleEdgeFilter.None);
                    g.DrawString(this.MainText, this.Font, Brushes.White, mainAreaBackground, f);

                    // Draw right
                    if (RightBarSize > 0)
                    {
                        g.FillRoundedRectangle(_LeftAndRightBrush, rightArea, this.RoundedCornerAngle, RectangleEdgeFilter.TopRight | RectangleEdgeFilter.BottomRight);
                        g.DrawString(this.RightText, this.Font, Brushes.White, rightArea, f);
                    }
                }
            }
           * */
        }

      

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DraggableUserControl
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Name = "DraggableUserControl";
            this.Size = new System.Drawing.Size(294, 36);
            this.ResumeLayout(false);

        }
    }
}