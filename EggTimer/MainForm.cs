using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EggTimer
{
    public partial class MainForm : Form
    {
        public DateTime dtAlertTime;

        public MainForm()
        {
            InitializeComponent();
            dtAlertTime = new DateTime();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            nudHours.Enabled = false;
            nudMinutes.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            dtAlertTime = DateTime.Now
                .AddHours((double)nudHours.Value)
                .AddMinutes((double)nudMinutes.Value);
            tmrUpdate.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (dtAlertTime.Hour == DateTime.Now.Hour &&
                dtAlertTime.Minute == DateTime.Now.Minute &&
                dtAlertTime.Second == DateTime.Now.Second)
            {
                Stop();
                Alert();
            }
            TimeSpan ts = dtAlertTime - DateTime.Now;
            lblCountdown.Text = String.Format("{0:D2}:{1:D2}:{2:D2}",
                ts.Hours, ts.Minutes, ts.Seconds);
        }

        private void Stop()
        {
            nudHours.Enabled = true;
            nudMinutes.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            tmrUpdate.Stop();
            lblCountdown.Text = "00:00:00";
        }

        private void Alert()
        {
            MessageBox.Show("Times Up!");
        }
    }
}