using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GLUITEST
{
    public partial class StartFrm : Form
    {
        public StartFrm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestAI efis = new TestAI();
            efis.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TestEfis efis = new TestEfis();
            efis.Show();
        }
    }
}
