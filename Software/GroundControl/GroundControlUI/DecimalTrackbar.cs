using System;
using System.ComponentModel;
using System.Windows.Forms;

/// Source: http://dotnet.itags.org/dotnet-tech/43688/
namespace TrackBarDecimals
{
    public class DecimalTrackbar : System.Windows.Forms.TrackBar
    {

        private System.ComponentModel.Container components = null;

        public DecimalTrackbar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void RecalcValue()
        {
            Value = (decimal)(base.Value * factor);
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            RecalcValue();
        }

        protected override void OnScroll(EventArgs e)
        {
            base.OnScroll(e);
            RecalcValue();
        }

        [Browsable(true)]
        public new decimal Maximum
        {
            get { return (decimal)(base.Maximum * factor); }
            set { base.Maximum = (int)(value / factor); }
        }

        [Browsable(true)]
        public new decimal Minimum
        {
            get { return (decimal)(base.Minimum * factor); }
            set { base.Minimum = (int)(value / factor); }
        }

        private decimal factor = 0.1M;
        [Browsable(true)]
        public decimal Factor
        {
            get { return factor; }
            set { factor = value; }
        }

        private decimal myValue;
        [Browsable(true)]
        public new decimal Value
        {
            get { return myValue; }
            set { myValue = value;
            base.Value = Convert.ToInt32(value / factor);
            }
        }

        [Browsable(true)]
        public new decimal TickFrequency
        {
            get { return (decimal)(base.TickFrequency * factor); }
            set { base.TickFrequency = (int)(value / factor); }
        }

        [Browsable(true)]
        public new decimal SmallChange
        {
            get { return (decimal)(base.SmallChange * factor); }
            set { base.SmallChange = (int)(value / factor); }
        }

        [Browsable(true)]
        public new decimal LargeChange
        {
            get { return (decimal)(base.LargeChange * factor); }
            set { base.LargeChange = (int)(value / factor); }
        }
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}