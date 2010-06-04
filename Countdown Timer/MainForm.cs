using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WMPLib;

namespace CountdownTimer
{
    public partial class MainForm : Form
    {
        DateTime dtAlertTime;
        WindowsMediaPlayer wmp = null;

        #region • Methods •

        private void PlayPause()
        {
            if (wmp.playState == WMPPlayState.wmppsPlaying)
            {
                wmp.controls.stop();
            }
            else if (System.IO.File.Exists(txtFilename.Text))
            {
                wmp.URL = txtFilename.Text;
                wmp.controls.play();
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

            foreach (Control ctrl in tableLayoutPanel2.Controls)
            {
                ctrl.Enabled = true;
            }
        }

        private void Alert()
        {
            Restore();
            if (chkAudio.Checked)
            {
                PlayPause();
            }
            if (MessageBox.Show(txtMessage.Text) == DialogResult.OK)
            {
                PlayPause();
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

        #endregion • Methods •

        #region • Event Handlers •


        #region MainForm

        public MainForm(string[] args)
        {
            InitializeComponent();
            dtAlertTime = new DateTime();
            wmp = new WindowsMediaPlayer();
            wmp.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(wmp_PlayStateChange);
            this.Icon = Properties.Resources.clock;
            niTray.Icon = Properties.Resources.clock;

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
                    else if (arg.StartsWith("-tm"))
                    {
                        nudMinutes.Value = int.Parse(arg.Substring(3));
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AppSettings Settings = AppSettings.Load();
            this.Location = Settings.Location;
            txtFilename.Text = Settings.AudioFilename;
            txtMessage.Text = Settings.MessageText;
            chkAudio.Checked = Settings.AudioAlert;
            chkHide.Checked = Settings.HideOnStart;

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
            AppSettings Settings = AppSettings.Load();
            Settings.Location = this.Location;
            Settings.AudioAlert = chkAudio.Checked;
            Settings.AudioFilename = txtFilename.Text;
            Settings.MessageText = txtMessage.Text;
            Settings.HideOnStart = chkHide.Checked;
            Settings.Save();
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

        #endregion MainWindow


        #region Buttons

        private void btnStart_Click(object sender, EventArgs e)
        {
            nudHours.Enabled = false;
            nudMinutes.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            foreach (Control ctrl in tableLayoutPanel2.Controls)
            {
                if (ctrl.Name != "btnNew")
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofdAudio.ShowDialog() == DialogResult.OK)
            {
                txtFilename.Text = ofdAudio.FileName;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayPause();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion Buttons


        #region Context Menu

        private void miRestore_Click(object sender, EventArgs e)
        {
            Restore();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion Context Menu


        #region TrayIcon

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

        #endregion TrayIcon


        #region Other Events

        void wmp_PlayStateChange(int NewState)
        {
            if (NewState == (int)WMPPlayState.wmppsPlaying)
                btnPlay.Text = "<";
            else if (NewState == (int)WMPPlayState.wmppsStopped)
                btnPlay.Text = "4";
            else
                btnPlay.Text = "4";
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = dtAlertTime - DateTime.Now;
            string msg = String.Format("{0:D2}:{1:D2}:{2:D2}",
                ts.Hours, ts.Minutes, ts.Seconds);

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

        #endregion Other Events


        #endregion • Event Handlers •

        #region • Add Button To Title Bar (Not working) •

        //[DllImport("user32.dll")]
        //public static extern IntPtr GetWindowDC(IntPtr hWnd);

        //[DllImport("user32.dll")]
        //public static extern IntPtr GetDC(IntPtr hWnd);

        //[DllImport("user32.dll")]
        //public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        //public const int WM_NCPAINT = 0x0085;


        //protected override void WndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case WM_NCPAINT:
        //            {
        //                base.WndProc(ref m);
        //                IntPtr hDC = GetWindowDC(m.HWnd);
        //                Graphics g = Graphics.FromHdc(hDC);
        //                PaintNC(m.HWnd);
        //                g.Dispose();
        //                ReleaseDC(m.HWnd, hDC);
        //                m.Result = IntPtr.Zero;
        //            }
        //            break;
        //        default:
        //            base.WndProc(ref m);
        //            break;
        //    }
        //}

        //protected void PaintNC(IntPtr hWnd)
        //{
        //    IntPtr hDC = GetWindowDC(hWnd);
        //    Graphics g = Graphics.FromHdc(hDC);
        //    int CaptionHeight = Bounds.Height - ClientRectangle.Height; // Title bar
        //    Size CloseButtonSize = SystemInformation.CaptionButtonSize;
        //    int X = Bounds.Width - 4 - CloseButtonSize.Width * 2;
        //    int Y = 6;
        //    ControlPaint.DrawButton(g, X, Y, 15, 14, ButtonState.Normal);
        //    g.Dispose();
        //    ReleaseDC(hWnd, hDC);
        //}

        #endregion • Add Button To Title Bar (Not working) •

    }
}