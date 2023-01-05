using dotnetCampus.FileDownloader;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;

namespace SJTOOLS
{
    public partial class Form2 : MaterialForm
    {
        string path = Application.StartupPath + "\\gnirehtet.exe";
        private BatStatus curBatSataus = BatStatus.NONE;
        private Process curProcess = new Process();
        public Form2()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Pink200, Primary.Pink100, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

        }

        public static void SystemNotify(string content,string title,Icon icon) 
        {
            //创建 NotifyIcon 实例
            NotifyIcon fyIcon = new NotifyIcon();
            fyIcon.Icon = icon;/*找一个ico图标将其拷贝到 debug 目录下*/
            fyIcon.BalloonTipText = content;/*必填提示内容*/
            fyIcon.BalloonTipTitle = title;
            fyIcon.Visible = true;/*必须设置显隐，因为默认值是 false 不显示通知*/
            fyIcon.ShowBalloonTip(0);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (File.Exists(path) == true)
            {
                if (curBatSataus != BatStatus.ON)
                {
                    curProcess.StandardInput.WriteLine("gnirehtet.exe autorun");
                    curBatSataus = BatStatus.ON;
                }
                else
                {
                    MessageBox.Show("当前进程正在运行，请先关闭");
                }
            }
            else
            {
                MessageBox.Show("未下载依赖！\n 不可启动", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Execute(true, "gnirehtet.exe stop");
            Execute(true, "taskkill -f -im gnirehtet.exe");
            if (curBatSataus == BatStatus.ON)
            {
                curProcess.CancelOutputRead();//取消异步操作
                curProcess.Kill();
                curBatSataus = BatStatus.OFF;
                //如果需要手动关闭，则关闭后再进行初始化
                InitInfo();
            }
        }
        private void InitInfo()
        {
            curProcess.OutputDataReceived -= new DataReceivedEventHandler(ProcessOutDataReceived);
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "cmd.exe";
            //p.Arguments = " -t 192.168.1.103";
            p.UseShellExecute = false;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            p.CreateNoWindow = true;
            p.RedirectStandardError = true;
            p.RedirectStandardInput = true;
            p.RedirectStandardOutput = true;
            curProcess.StartInfo = p;
            curProcess.Start();
            curProcess.BeginOutputReadLine();
            curProcess.OutputDataReceived += new DataReceivedEventHandler(ProcessOutDataReceived);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            InitInfo();
        }
        public void ProcessOutDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (this.richTextBox3.InvokeRequired)
            {
                this.richTextBox3.Invoke(new Action(() =>
                {
                    this.richTextBox3.AppendText(e.Data + "\r\n");
                }));
            }
            else
            {
                this.richTextBox3.AppendText(e.Data + "\r\n");
            }
        }
        public static void Execute(bool NoWindow, string command)
        {
            System.Diagnostics.Process process = new();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c" + command;
            process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
            process.StartInfo.CreateNoWindow = NoWindow;//不显示程序窗口
            process.Start();
            process.WaitForExit(); //等待程序执行完退出进程
            process.Close();
        }
        public enum BatStatus
        {
            NONE = 0,
            ON = 1,
            OFF = 2
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(this.curProcess.StartInfo.FileName) || this.curProcess.StandardInput.BaseStream.CanWrite) && curBatSataus != BatStatus.OFF)
            {
                curBatSataus = BatStatus.OFF;
                richTextBox3.ScrollToCaret();
            }
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            curProcess.CancelOutputRead();//取消异步操作
            curProcess.Kill();
            Execute(true, "gnirehtet.exe stop & taskkill -f -im gnirehtet.exe");
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("即将进入后台下载，请等待系统通知","",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            FileInfo fileInfo = new FileInfo(path);
            var SegDownloader = new SegmentFileDownloader("https://pan.konon.top/zahuo/gnirehtet.exe", fileInfo);
            await SegDownloader.DownloadFileAsync();
            if (SegDownloader.File.Exists == true)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
                SystemNotify("下载完成！", "通知", (Icon)(resources.GetObject("$this.Icon")));
            }
        }
        private async void button7_Click(object sender, EventArgs e)
        {
            string instpath = Application.StartupPath + "\\gnirehtet.apk";
            if (File.Exists(instpath) == true)
            {
                if (Form1.InvokeExcute(true, "adb shell pm list packages").Contains("com.genymobile.gnirehtet") != true)
                {
                    if (Form1.InvokeExcute(true, String.Format(@"adb pm install -r ""{0}""", instpath)).Contains("Success") == true)
                    {
                        MessageBox.Show("安装成功！");
                    }
                }
                else
                {
                    MessageBox.Show("已安装，无需安装！");
                }
            }
            else
            {
                FileInfo fileInfo = new FileInfo(instpath);
                var SegDownloader = new SegmentFileDownloader("https://pan.konon.top/zahuo/gnirehtet.apk", fileInfo);
                await SegDownloader.DownloadFileAsync();
                if (Form1.InvokeExcute(true, "adb shell pm list packages").Contains("com.genymobile.gnirehtet") != true)
                {
                    if (Form1.InvokeExcute(true, String.Format(@"adb pm install -r ""{0}""", instpath)).Contains("Success") == true)
                    {
                        MessageBox.Show("安装成功！");
                    }
                    else
                    {
                        MessageBox.Show("安装失败！");
                    }
                }
                else
                {
                    MessageBox.Show("已安装，无需安装！");
                }
            }
        }
    }
}
