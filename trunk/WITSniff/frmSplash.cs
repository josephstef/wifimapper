using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WITSniff
{
    public partial class frmSplash : Form
    {
        Timer Clock;
        int fadeCount = 0;
        public frmSplash()
        {
            InitializeComponent();
            lblName.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Clock = new System.Windows.Forms.Timer();
            Clock.Interval = 1;
            Clock.Start();
            Clock.Tick += new EventHandler(Timer_Tick);

        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == Clock)
                this.Opacity = fadeCount + 1;
            if (fadeCount == 100)
            {
                Clock.Stop();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void frmSplash_Load(object sender, EventArgs e)
        {

        }

    }
}
