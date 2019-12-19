using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PHP_Switch
{
    public partial class Form1 : Form
    {

        public string PHP_5_Module = "LoadModule php5_module \"c:/php5/php5apache2_4.dll\"";
        public string PHP_7_Module = "LoadModule php7_module \"c:/php/php7apache2_4.dll\"";

        public string PHP_5_Dir = "PHPIniDir \"C:/php5\"";
        public string PHP_7_Dir = "PHPIniDir \"C:/php\"";

        public string httpConfPath = "C:/Apache24/conf/httpd.conf";

        public Form1()
        {
            InitializeComponent();

            InitializeTimer();

            watchPHPVersion();

            radioButton1.CheckedChanged += new EventHandler(RadioButtonCheckedChanged);
            radioButton1.CheckedChanged += new EventHandler(RadioButtonCheckedChanged);

            button1.Enabled = false;
        }

        private void RadioButtonCheckedChanged(object Sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void changePHPVersion()
        {
            string text = File.ReadAllText(httpConfPath);
            ServiceController sc = new ServiceController("ApacheServer");

            if (radioButton1.Checked == true)
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    text = text.Replace(PHP_7_Module, PHP_5_Module);
                    text = text.Replace(PHP_7_Dir, PHP_5_Dir);
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(25));
                    sc.Start();
                }
                else if(sc.Status == ServiceControllerStatus.Stopped)
                {
                    text = text.Replace(PHP_7_Module, PHP_5_Module);
                    text = text.Replace(PHP_7_Dir, PHP_5_Dir);
                    sc.Start();
                }
            }

            if (radioButton2.Checked == true)
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    text = text.Replace(PHP_5_Module, PHP_7_Module);
                    text = text.Replace(PHP_5_Dir, PHP_7_Dir);
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(25));
                    sc.Start();
                }
                else if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    text = text.Replace(PHP_5_Module, PHP_7_Module);
                    text = text.Replace(PHP_5_Dir, PHP_7_Dir);
                    sc.Start();
                }
            }

            File.WriteAllText(httpConfPath, text);

            button1.Enabled = false;
        }

        private void watchPHPVersion()
        {
            string line;

            StreamReader file = new StreamReader(httpConfPath);

            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(PHP_5_Module))
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                    break;
                } 
                else if (line.Contains(PHP_7_Module))
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                    break;
                }
            }

            file.Close();
        }


        private void InitializeTimer()
        {          
            timer1.Interval = 500;
            timer1.Tick += new EventHandler(Timer1_Tick);

            // Enable timer.  
            timer1.Enabled = true;
        }

        private void Timer1_Tick(object Sender, EventArgs e)
        {
            ServiceController sc = new ServiceController("ApacheServer");

            if (sc.Status == ServiceControllerStatus.Running)
            {
                label1.Text = "Apache is running.";
                pictureBox1.Image = PHP_Switch.Properties.Resources.accept_circle_100;
                button2.Enabled = false;
                button3.Enabled = true;
            }

            else if (sc.Status == ServiceControllerStatus.Stopped)
            {
                label1.Text = "Apache is stopped.";
                pictureBox1.Image = PHP_Switch.Properties.Resources.remove_circle_100;
                button2.Enabled = true;
                button3.Enabled = false;
            }
            else
            {
                label1.Text = "Status pending...";
                pictureBox1.Image = PHP_Switch.Properties.Resources.more_circle_100;
                button2.Enabled = false;
                button3.Enabled = false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            changePHPVersion();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServiceController sc = new ServiceController("ApacheServer");
            sc.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServiceController sc = new ServiceController("ApacheServer");
            sc.Stop();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form form2 = new options();
            form2.Show();
        }
    }
}
