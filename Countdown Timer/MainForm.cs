using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;

namespace CountdownTimer
{
    public partial class MainForm : Form
    {
        public DateTime dtAlertTime;
        public SoundPlayer sp;

        public MainForm(string[] args)
        {
            InitializeComponent();
            dtAlertTime = new DateTime();
            sp = new SoundPlayer();
            txtMessage.Text = Properties.Settings.Default.MessageText;
            nudHours.Value = 0;
            nudMinutes.Value = 0;

            foreach (string arg in args)
            {
                try
                {
                    if (arg.StartsWith("-m"))
                    {
                        txtMessage.Text = arg.Substring(2);
                    }
                    else if (arg.StartsWith("-th"))
                    {
                        nudHours.Value = int.Parse(arg.Substring(3));
                    }
                    else if (arg.StartsWith("-th"))
                    {
                        nudMinutes.Value = int.Parse(arg.Substring(3));
                    }
                }
                catch (Exception ex)
                {
                    // Do Nothing
                }
            }

            chkAudio.Checked = Properties.Settings.Default.AudioAlert;
            txtFilename.Text = Properties.Settings.Default.AudioFilename;

            switch (chkAudio.CheckState)
            {
                case CheckState.Checked:
                    txtFilename.Enabled = true;
                    btnBrowse.Enabled = true;
                    btnPlay.Enabled = true;
                    break;
                case CheckState.Unchecked:
                    txtFilename.Enabled = false;
                    btnBrowse.Enabled = false;
                    btnPlay.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.AudioAlert = chkAudio.Checked;
            Properties.Settings.Default.AudioFilename = txtFilename.Text;
            Properties.Settings.Default.MessageText = txtMessage.Text;
            Properties.Settings.Default.Save();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            nudHours.Enabled = false;
            nudMinutes.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            foreach (Control ctrl in groupBox1.Controls)
            {
                ctrl.Enabled = false;
            }

            dtAlertTime = DateTime.Now
                .AddHours((double)nudHours.Value)
                .AddMinutes((double)nudMinutes.Value);
            tmrUpdate.Start();

            if (chkHide.Checked)
            {
                Minimize();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = dtAlertTime - DateTime.Now;
            string msg = String.Format("{0:D2}:{1:D2}:{2:D2} - {3}",
                ts.Hours, ts.Minutes, ts.Seconds, txtMessage.Text);
            
            lblCountdown.Text = msg;
            niTray.BalloonTipText = msg;
            niTray.Text = msg;
            
            if (dtAlertTime.Hour == DateTime.Now.Hour &&
                dtAlertTime.Minute == DateTime.Now.Minute &&
                dtAlertTime.Second == DateTime.Now.Second)
            {
                Stop();
                Alert();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofdAudio.ShowDialog() == DialogResult.OK)
            {
                txtFilename.Text = ofdAudio.FileName;
            }
        }

        private void chkAudio_CheckedChanged(object sender, EventArgs e)
        {
            switch (chkAudio.CheckState)
            {
                case CheckState.Checked:
                    txtFilename.Enabled = true;
                    btnBrowse.Enabled = true;
                    btnPlay.Enabled = true;
                    break;
                case CheckState.Unchecked:
                    txtFilename.Enabled = false;
                    btnBrowse.Enabled = false;
                    btnPlay.Enabled = false;
                    break;
                default:
                    break;
            }

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Minimize();
            }
            else
            {
                Restore();
            }
        }

        private void miRestore_Click(object sender, EventArgs e)
        {
            Restore();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Play()
        {
            if (System.IO.File.Exists(txtFilename.Text))
            {
                sp.SoundLocation = txtFilename.Text;
                sp.Play();
            }
        }

        private void Stop()
        {
            nudHours.Enabled = true;
            nudMinutes.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            tmrUpdate.Stop();
            lblCountdown.Text = "00:00:00";

            foreach (Control ctrl in groupBox1.Controls)
            {
                ctrl.Enabled = true;
            }
        }

        private void Alert()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Restore();
            }
            MessageBox.Show(txtMessage.Text);
            if (chkAudio.Checked)
            {
                Play();
            }
        }

        private void Restore()
        {
            this.Visible = true;
            niTray.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void Minimize()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            niTray.Visible = true;
        }

        private void niTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Restore();
        }

        private void niTray_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                niTray.ShowBalloonTip(5000);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(Application.ExecutablePath);
                Process.Start(Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}