using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchDogeSync
{
    public partial class Form1 : Form
    {
        private bool exit = false, hide = false;

        public Form1()
        {
            InitializeComponent();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                System.Threading.Thread.Sleep(60000);
                for (int i = 0; i < 10; i++)
                {
                    if (SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) == 0)
                    {
                        labelStatus.Text = "Time set by power up event";
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            if (SetTime((int)numH.Value, (int)numM.Value, (int)numS.Value) == 0)
            {
                labelStatus.Text = "Time set";
            }
            else
            {
                labelStatus.Text = "Failed to set time";
            }
        }

        private void buttonSync_Click(object sender, EventArgs e)
        {
            if (SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) == 0)
            {
                labelStatus.Text = "Time synced";
            }
            else
            {
                labelStatus.Text = "Failed to sync time";
            }
        }

        private void checkBoxSync_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSync.Checked)
            {
                timerSync.Start();
            }
            else
            {
                timerSync.Stop();
            }
        }

        private int SetTime(int h, int m, int s)
        {
            try
            {
                byte[] buf = new byte[32];
                serialPort1.PortName = textPort.Text;
                serialPort1.Open();
                buf[0] = 254;
                buf[1] = (byte)h;
                buf[2] = (byte)m;
                buf[3] = (byte)s;
                serialPort1.Write(buf, 0, 4);
                serialPort1.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        private void timerSync_Tick(object sender, EventArgs e)
        {
            if (SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) == 0)
            {
                labelStatus.Text = "Time synced by timer";
            }
            else
            {
                labelStatus.Text = "Failed to sync time by timer";
            }
        }

        private void notifyIconTray_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exit)
            {
                e.Cancel = true;
                Visible = false;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Visible = true;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            exit = true;
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if(hide)
            {
                Hide();
                hide = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                textPort.Text = args[1];
                serialPort1.PortName = textPort.Text;
                serialPort1.DtrEnable = false;
                SetTime(0, 0, 0);
                System.Threading.Thread.Sleep(2000);
                if (SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) == 0)
                {
                    labelStatus.Text = "Time set at app start";
                    checkBoxSync.Checked = true;
                    hide = true;
                }
                else
                {
                    MessageBox.Show("Cannot sync time. Application will be shut down.", "WatchDogeSync", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height;
            Left = Screen.PrimaryScreen.WorkingArea.Width - Width;
        }
    }
}
