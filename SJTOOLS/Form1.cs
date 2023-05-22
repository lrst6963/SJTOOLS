using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using dotnetCampus.FileDownloader;
using System.Threading.Tasks;
using System.Threading;

namespace SJTOOLS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
}
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.WParam.ToInt32() == 7)
                {
                    //MessageBox.Show("设备状态发生了变化！");
                    string strss = StringFG(InvokeExcute(true, "fastboot devices"), "&exit\r\n").Replace("\tfastboot", string.Empty);
                    string[] striparr = strss.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    striparr = striparr.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    comboBox3.Items.Clear();
                    label17.Text = string.Format("已连接{0}台设备", striparr.Length.ToString());
                    for (int i = 0; i < striparr.Length; i++)
                    {
                        comboBox3.Items.Add(striparr[i]);
                        comboBox3.Text = striparr[0];
                        if (Fbdevice.Length <= 8)
                        {
                            Fbdevice = string.Empty;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }
        ComponentResourceManager resources = new(typeof(Form1));
        String[] strings = new String[12];
        string Fbdevice = string.Empty;
        internal object richTextBox1;
        internal object isRun;

        private void Form1_Load(object sender, EventArgs e)
        {
            string strss = StringFG(InvokeExcute(true, "fastboot devices"), "&exit\r\n").Replace("\tfastboot", string.Empty);
            string[] striparr = strss.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            striparr = striparr.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            comboBox3.Items.Clear();
            label17.Text = string.Format("已连接{0}台设备", striparr.Length.ToString());
            for (int i = 0; i < striparr.Length; i++)
            {
                comboBox3.Items.Add(striparr[i]);
                comboBox3.Text = striparr[0];
            }
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 0;
            strings[0] = label4.Text;
            strings[1] = label5.Text;
            strings[2] = label6.Text;
            strings[3] = label7.Text;
            strings[4] = label8.Text;
            strings[5] = label9.Text;
            strings[6] = label10.Text;
            strings[7] = label11.Text;
            strings[8] = label12.Text;
            strings[9] = label13.Text;
            strings[10] = label14.Text;
            strings[11] = label15.Text;
            Opacity = 0.9F;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 1:
                    if (InvokeExcute(true, @"adb reboot").Contains("no devices") == true)
                    {
                        mess_error();
                    }
                    else
                    {
                        mess_done();
                    }
                    break;
                case 2:
                    if (InvokeExcute(true, @"adb reboot recovery").Contains("no devices") == true)
                    {
                        mess_error();
                    }
                    else
                    {
                        mess_done();
                    }
                    break;
                case 3:
                    if (InvokeExcute(true, @"adb reboot bootloader").Contains("no devices") == true)
                    {
                        mess_error();
                    }
                    else
                    {
                        mess_done();
                    }
                    break;
                case 0:
                    if (InvokeExcute(true, @"adb shell reboot -p").Contains("no devices") == true)
                    {
                        mess_error();
                    }
                    else
                    {
                        mess_done();
                    }
                    break;
                case 4:
                    if (InvokeExcute(true, @"adb shell edl").Contains("no devices") == true)
                    {
                        mess_error();
                    }
                    else
                    {
                        mess_done();
                    }
                    break;
                default:
                    MessageBox.Show("请选择选项");
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string clicks = InvokeExcute(true, @"adb devices");
            if (clicks.Contains("recovery") == true)
            {
                MessageBox.Show("设备已连接\nRecovery模式");
            }
            else if (clicks.Contains("sideload") == true)
            {
                MessageBox.Show("设备已连接\nSideload模式");
            }
            else if (Devices_check() == true)
            {
                MessageBox.Show("设备已连接\n系统模式");
            }
            else
            {
                MessageBox.Show("设备未连接");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != String.Empty)
            {
                if (Devices_check() == true)
                {
                    MessageBox.Show(InvokeExcute(false, string.Format(@"{0}", textBox1.Text)));
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "双击选择文件")
            {
                if (Devices_check() == true)
                {
                    string filename = Path.GetFileName(textBox2.Text);
                    string renames = filename.Replace(" ", "_");
                    Execute(false, String.Format(@"adb push ""{0}"" /sdcard/adbtmp & adb shell mv /sdcard/adbtmp ""/sdcard/{1}"" & exit", textBox2.Text, renames));
                    MessageBox.Show(String.Format("执行成功！\n文件传输到/sdcard/{0}", filename));
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "双击选择文件")
            {
                string clicks = InvokeExcute(true, @"adb devices");
                if (clicks.Contains("recovery") != true & Sideload_check() != true)
                {
                    MessageBox.Show("手机未处于Recovery模式", "警告");
                }
                else
                {
                    Execute(true, "adb shell twrp sideload");
                    Thread.Sleep(400);
                    Execute(false, string.Format(@"adb sideload ""{0}""", textBox3.Text));
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            Label[] labels = new Label[12];
            labels[0] = label4;
            labels[1] = label5;
            labels[2] = label6;
            labels[3] = label7;
            labels[4] = label8;
            labels[5] = label9;
            labels[6] = label10;
            labels[7] = label11;
            labels[8] = label12;
            labels[9] = label13;
            labels[10] = label14;
            labels[11] = label15;
            if (Devices_check() == true)
            {
                if (InvokeExcute(true, @"adb shell ""pm list features | grep host""").Contains("android.hardware.usb.host") == true)
                {
                    labels[8].Text = "是否支持OTG：是";
                }
                else
                {
                    label12.Text = "是否支持OTG：否";
                }
                for (int i = 0; i < 12; i++)
                {
                    if (i != 8)
                    {
                        labels[i].Text = strings[i];
                        labels[i].Text += GetPhoneInfo(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    labels[i].Text = strings[i];
                    labels[i].Text += "设备未连接";
                }
            }
            button6.Enabled = true;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("手机已解除BL锁。\n\n已刷入包含ADB或文件管理的第三方Recovery。\n\n已加密Data分区的手机无法操作（或者将Data分区解密）。\n\n熟悉在手机出现无法开机等意外情况的补救。\n\n！！谨慎操作，可能导致无法开机。！！", "前提条件", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                string clicks = InvokeExcute(true, @"adb devices");
                if (clicks.Contains("recovery") == true)
                {
                    string path = string.Format("{0}\\rmlock.sh", Environment.CurrentDirectory);
                    if (!File.Exists(path))
                    {
                        // 创建一个要写入的文件。
                        string[] value = {
                    "#!/bin/sh",
                    "rm /data/system/locksettings.db",
                    "rm /data/system/locksettings.db-shm",
                    "rm /data/system/locksettings.db-wal",
                    "rm /data/system/gatekeeper.password.key",
                    "rm /data/system/gatekeeper.pattern.key",
                    @"if [ ""$?"" == ""0"" ];then",
                    @"echo ""dones""; else",
                    @"echo ""114514""; fi"
                };
                        string[] createText = value;
                        File.WriteAllLines(path, createText, Encoding.UTF8);
                    }
                    Execute(true, string.Format(@"adb push ""{0}"" /cache/rmlock.sh", path));
                    if (InvokeExcute(true, @"adb shell ""sh /cache/rmlock.sh""").Contains("dones") == true)
                    {
                        mess_done();
                    }
                    else
                    {
                        MessageBox.Show("执行错误", "ERROR");
                    }
                }
                else
                {
                    MessageBox.Show("设备未处于Recovery模式", "ERROR");
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Execute(true, "taskkill -f -im adb.exe");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Button but = new Button();// 创建button按钮类
            but.Name = "but";//给按钮设置名字
            but.Text = "A";//给按钮设置标题
            but.Click += new EventHandler(but_Click);//给按钮添加click事件
            but.Size = new Size(100, 100);// 设置按钮的大小
            but.Location = new Point(50, 20);//设置按钮的位置
            but.Font = new System.Drawing.Font("Microsoft YaHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            Button but1 = new Button();// 创建button按钮类
            but1.Name = "but1";//给按钮设置名字
            but1.Text = "B";//给按钮设置标题
            but1.Click += new EventHandler(but_Click);//给按钮添加click事件
            but1.Size = new Size(100, 100);// 设置按钮的大小
            but1.Location = new Point(150, 20);//设置按钮的位置
            but1.Font = new System.Drawing.Font("Microsoft YaHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            Button but2 = new Button();// 创建button按钮类
            but2.Name = "but2";//给按钮设置名字
            but2.Text = "重启";//给按钮设置标题
            but2.Click += new EventHandler(but_Click);//给按钮添加click事件
            but2.Size = new Size(100, 30);// 设置按钮的大小
            but2.Location = new Point(100, 130);//设置按钮的位置
            but2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);


            //创建和设置新窗口↓
            Form winform = new Form();//创建Form窗口类
            winform.Load += new EventHandler(winform_Load);//给窗口加load事件
            winform.FormBorderStyle = FormBorderStyle.Fixed3D;
            winform.Text = "选择分区";//给窗口设置标题
            winform.Height = 220;//给窗口设置高度
            winform.Width = 320;//给窗口设置宽度
            winform.Controls.Add(but);//给窗口添加设置好的按钮
            winform.Controls.Add(but1);
            winform.Controls.Add(but2);
            winform.ShowIcon = true;// 设置窗口的lcon是否显示
            winform.MaximizeBox = false;
            winform.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            winform.Icon = (Icon)resources.GetObject("$this.Icon");
            winform.ShowDialog(); //显示窗口显示形式父窗口不能活动
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(StringFG(InvokeExcute(true, "fastboot devices"), "&exit\r\n").Replace("\tfastboot",string.Empty));
            if (Fastboot_Check() == true)
            {
                MessageBox.Show(string.Format("找到设备：{0}", StringFG(InvokeExcute(true, "fastboot devices"), "&exit\r\n").Replace("\tfastboot", string.Empty)));
            }
            else
            {
                mess_error();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (Fbdevice != string.Empty)
            {
                DialogResult DialogResults = MessageBox.Show(string.Format(@"确定擦除""{0}"" 分区吗？", comboBox2.Text), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (DialogResults == DialogResult.Yes)
                {
                    if (InvokeExcute(true, string.Format(@"fastboot -s {0} erase ""{1}""", Fbdevice, comboBox2.Text)).Contains("OKAY") == true)
                    {
                        mess_done();
                    }
                    else
                    {
                        MessageBox.Show("擦除失败");
                    }
                }
            }
            else
            {
                MessageBox.Show("未选择设备！");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text != string.Empty)
            {
                Fbdevice = comboBox3.Text;
                BL_lock_Check();
                userdata_Check();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Form2 form = new();
            form.Show();
        }

        private async void button14_ClickAsync(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\adb-setup-1.4.2.exe";
            FileInfo fileInfo = new FileInfo(path);
            var segmentFileDownloader = new SegmentFileDownloader("https://pan.konon.top/zahuo/adb-setup-1.4.2.exe", fileInfo);
            Form2.SystemNotify("进入后台下载！", "通知", (Icon)(resources.GetObject("$this.Icon")));
            await segmentFileDownloader.DownloadFileAsync();
            if (segmentFileDownloader.File.Exists == true)
            {
                DialogResult dialogResult = MessageBox.Show("下载完成！\n现在安装？", "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.OK)
                {
                    Execute(true, path);
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "双击选择文件")
            {
                if (Devices_check() != true)
                {
                    MessageBox.Show("手机未处于系统模式", "警告");
                }
                else
                {
                    InvokeExcute(false, string.Format(@"echo 安装中... &adb install -r ""{0}""", textBox4.Text));
                    MessageBox.Show("执行完成，请检查安装结果！");
                }
            }
        }

        private void button1617_Click(object sender, EventArgs e)
        {
            if (Fbdevice != string.Empty)
            {
                if ((Button)sender == button16)
                {
                    DialogResult DialogResults = MessageBox.Show("确定启动吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (DialogResults == DialogResult.Yes)
                    {
                        if (InvokeExcute(true, string.Format(@"fastboot -s {0} boot ""{1}""", Fbdevice, textBox5.Text)).Contains("OKAY") == true)
                        {
                            mess_done();
                        }
                        else
                        {
                            MessageBox.Show("启动失败！");
                        }
                    }
                }
                else if ((Button)sender == button17)
                {
                    DialogResult DialogResults = MessageBox.Show("确定刷入吗？\n\n请勿更改img镜像默认名，例如logo.img | system.img，修改默认名将导致镜像刷入失败!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (DialogResults == DialogResult.Yes)
                    {
                        string blocknames = Path.GetFileName(textBox6.Text).Replace(".img", string.Empty);
                        if (InvokeExcute(true, string.Format(@"fastboot -s {0} flash {1} ""{2}""", Fbdevice, blocknames, textBox6.Text)).Contains("OKAY") == true)
                        {
                            mess_done();
                        }
                        else
                        {
                            MessageBox.Show("刷入失败！");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("未选择设备！");
            }
        }
        public static bool Sideload_check()
        {
            if (InvokeExcute(true, @"adb devices").Contains("sideload") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool Devices_check()
        {
            if (StringFG(InvokeExcute(true, string.Format("adb devices")), "attached\r\n").Contains("device") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool Fastboot_Check()
        {
            if (StringFG(InvokeExcute(true, "fastboot devices"), "&exit\r\n").Contains("fastboot") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string openFileDialog(string name)
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = name;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;

            }
            return file;
        }
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "双击选择文件")
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).ForeColor = Color.Black;
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {

            if (((TextBox)sender).Text == string.Empty)
            {
                ((TextBox)sender).Text = "双击选择文件";
                ((TextBox)sender).ForeColor = Color.LightGray;

            }
        }
        private void textBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (((TextBox)sender) == textBox2)
            {
                ((TextBox)sender).Text = openFileDialog("所有文件(*.*)|*.*");
            }
            else if (((TextBox)sender) == textBox3)
            {
                ((TextBox)sender).Text = openFileDialog("压缩文件(*.zip)|*.zip");
            }
            else if (((TextBox)sender) == textBox4)
            {
                ((TextBox)sender).Text = openFileDialog("APK文件(*.apk)|*.apk");
            }
            else if (((TextBox)sender) == textBox5 | ((TextBox)sender) == textBox6)
            {
                ((TextBox)sender).Text = openFileDialog("IMG文件(*.img)|*.img");
            }
            ((TextBox)sender).ForeColor = Color.Black;
        }
        public void Execute(bool NoWindow, string command)
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
        public static string InvokeExcute(bool NoWindow, string Command)
        {
            Command = Command.Trim().TrimEnd('&') + "&exit";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = NoWindow;          //不显示程序窗口
                p.Start();//启动程序
                          //向cmd窗口写入命令
                p.StandardInput.WriteLine(Command);
                p.StandardInput.AutoFlush = true;
                //获取cmd窗口的输出信息
                StreamReader reader = p.StandardOutput;//截取输出流
                StreamReader error = p.StandardError;//截取错误信息
                string str = reader.ReadToEnd() + error.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                return str;
            }
        }
        public void mess_error() { MessageBox.Show("错误，没有找到设备!", "ERROR"); }
        public void mess_done() { MessageBox.Show("执行成功!", "Successfully"); }
        public static string GetPhoneInfo(int Getprops)
        {
            string exstr = "";
            switch (Getprops)
            {
                case 0:     //SDK 版本
                    exstr = "ro.build.version.sdk";
                    break;
                case 1:     //Android 系统版本
                    exstr = "ro.build.version.release";
                    break;
                case 2:     //Android 安全补丁程序级别
                    exstr = "ro.build.version.security_patch";
                    break;
                case 3:     //型号
                    exstr = "ro.product.model";
                    break;
                case 4:     //品牌
                    exstr = "ro.product.brand";
                    break;
                case 5:     //设备名
                    exstr = "ro.product.name";
                    break;
                case 6:     //处理器型号
                    exstr = "ro.product.board";
                    break;
                case 7:     //CPU 支持的 abi 列表
                    exstr = "ro.product.cpu.abilist";
                    break;
                case 8:     //是否支持 OTG
                    exstr = "persist.sys.isUsbOtgEnabled";
                    break;
                case 9:    //屏幕密度
                    exstr = "ro.sf.lcd_density";
                    break;
                case 10:    //每个应用程序的内存上限
                    exstr = "dalvik.vm.heapsize";
                    break;
                case 11:    //序列号
                    exstr = "ro.serialno";
                    break;
                default:
                    break;
            }
            string rei = InvokeExcute(true, string.Format("adb shell getprop {0}", exstr));
            string fn = rei;
            string reinfo = string.Empty;
            string[] words = fn.Split("exit\r\n");
            foreach (var word in words)
            {
                reinfo = word;
            }
            return reinfo;
        }
        public static string StringFG(string input, string fgs)
        {
            string rei = input;
            string fn = rei;
            string reinfo = string.Empty;
            string[] words = fn.Split(fgs);
            foreach (var word in words)
            {
                reinfo = word;
            }
            return reinfo;
        }
        public void but_Click(object sender, EventArgs e) //新窗口的按钮的单击事件
        {
            if (Fbdevice != string.Empty)
            {
                if (((Button)sender).Text == "A")
                {
                    if (InvokeExcute(true, "fastboot.exe -s " + Fbdevice + " --set-active=a").Contains("OKAY") == true)
                    {
                        mess_done();
                    }
                    else
                    {
                        MessageBox.Show("失败", "ERROR");
                    }
                }
                else if (((Button)sender).Text == "B")
                {
                    if (InvokeExcute(true, "fastboot.exe -s " + Fbdevice + " --set-active=b").Contains("OKAY") == true)
                    {
                        mess_done();
                    }
                    else
                    {
                        MessageBox.Show("失败", "ERROR");
                    }
                }
                else
                {
                    if (InvokeExcute(true, "fastboot.exe -s " + Fbdevice + " reboot").Contains("OKAY") == true)
                    {
                        mess_done();
                    }
                    else
                    {
                        MessageBox.Show("失败", "ERROR");
                    }
                }
            }
            else
            {
                MessageBox.Show("未选择设备！");
            }
        }
        public void winform_Load(object sender, EventArgs e)//新窗口的加载事件
        {

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 5)
            {
                comboBox2.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox2.Text = "";
            }
            else
            {
                comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float f = (float)trackBar1.Value;
            f = f * 0.01F;
            this.Opacity = f;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("退出程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            this.Text = "ADB and FASTBOOT Toolss                       " + StringFG(date.ToString(), " ");
        }
        public void BL_lock_Check()
        {
            string ss = InvokeExcute(true, String.Format("fastboot -s {0} oem device-info", Fbdevice));
            ss = StringFG(ss, "Verity mode: true\r\n");
            if (ss.Contains("(bootloader) Device unlocked: true") == true)
            {
                label20.Text = "BL锁状态：解锁";
                label21.ImageIndex = 0;
            }
            else
            {
                label20.Text = "BL锁状态：上锁";
                label21.ImageIndex = 1;
            }
        }
        public void userdata_Check() 
        {
            string ss = InvokeExcute(true, String.Format("fastboot -s {0} getvar partition-size:userdata", Fbdevice));
            ss = StringFG(ss, "partition-size:userdata:").Replace(" ","").Replace("\r\n","");
            string s = ss.Remove(ss.Length-25);
            ulong shi = Convert.ToUInt64(s, 16);
            for (int i = 0; i < 3; i++)
            {
                shi = shi / 1024;
            }
            label22.Text = "用户数据分区大小：" + shi.ToString() + "GB";
        }
        private void button18_Click(object sender, EventArgs e)
        {
            if (Fbdevice != String.Empty)
            {
                string ss = InvokeExcute(false, String.Format("fastboot -s {0} oem device-info", Fbdevice));
                ss = StringFG(ss, "Verity mode: true\r\n");
                if (ss.Contains("(bootloader) Device unlocked: true") == true)
                {
                    MessageBox.Show("Bootloader处于解锁状态！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Bootloader处于上锁状态！\n 是否进行解锁？\n 此解锁只能解部分手机(如OnePlus)", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (InvokeExcute(false, String.Format("fastboot -s {0} oem unlock", Fbdevice)).Contains("OKAY") == true)
                        {
                            mess_done();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("未选择设备！");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (Fbdevice != String.Empty)
            {
                if (((Button)sender) == button19)
                {
                    Execute(true, "fastboot reboot");
                }
                else if (((Button)sender) == button20)
                {
                    Execute(true, "fastboot reboot-bootloader");
                }
            }
            else
            {
                MessageBox.Show("未选择设备！");
            }
        }


    }
}
