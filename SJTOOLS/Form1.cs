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
            for (int i = 0; i < 12; i++)
            {
                var label = Controls.Find($"label{i + 4}", true).FirstOrDefault() as Label;
                if (label != null)
                {
                    strings[i] = label.Text;
                }
            }

            Opacity = 0.9F;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 验证设备是否连接
            if (!Devices_check())
            {
                mess_error();
                return;
            }
            // 使用字典映射操作命令，提高可读性和可维护性
            var commands = new Dictionary<int, string>
            {
                { 0, "shell reboot -p" },  // 关机
                { 1, "reboot" },           // 重启
                { 2, "reboot recovery" },  // 重启到Recovery
                { 3, "reboot bootloader" },// 重启到Bootloader
                { 4, "shell edl" }         // 进入EDL模式
            };
            // 检查是否选择了有效选项
            if (!commands.TryGetValue(comboBox1.SelectedIndex, out var command))
            {
                MessageBox.Show("请选择选项");
                return;
            }
            // 执行命令并处理结果
            string output = InvokeExcute(true,"adb " + command);
            MessageBox.Show(output);
            if (output.Contains("no devices"))
            {
                mess_error();
            }
            else
            {
                mess_done();
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
            // 验证文件选择
            var DefaultFileText = "双击选择文件";
            if (textBox2.Text == DefaultFileText || !File.Exists(textBox2.Text))
            {
                MessageBox.Show("请先选择有效的文件");
                return;
            }
            if (!Devices_check())
            {
                mess_error();
                return;
            }
            try
            {
                button4.Enabled = false; // 禁用按钮防止重复点击
                string fileName = Path.GetFileName(textBox2.Text);
                string safeFileName = fileName.Replace(" ", "_");
                // 构建传输命令
                string pushCommand = $@"adb push ""{textBox2.Text}"" /sdcard/adbtmp";
                string moveCommand = $@"adb shell mv /sdcard/adbtmp ""/sdcard/{safeFileName}""";

                // 异步执行传输
                await Task.Run(() =>
                {
                    Execute(false,pushCommand);
                    Execute(false,moveCommand);
                });
                MessageBox.Show($"文件传输成功！\n路径: /sdcard/{fileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"文件传输失败: {ex.Message}");
            }
            finally
            {
                button4.Enabled = true; // 恢复按钮状态
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

        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                button6.Enabled = false;

                Label[] labels =
                [
                    label4, label5, label6, label7,
            label8, label9, label10, label11,
            label12, label13, label14, label15
                ];

                if (!Devices_check())
                {
                    UpdateLabelsForDisconnectedDevice(labels);
                    return;
                }

                await CheckOtgSupportAndUpdateLabels(labels);
            }
            finally
            {
                button6.Enabled = true;
            }
        }

        private void UpdateLabelsForDisconnectedDevice(Label[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = $"{strings[i]}设备未连接";
            }
        }

        private async Task CheckOtgSupportAndUpdateLabels(Label[] labels)
        {
            bool hasOtgSupport = await Task.Run(() =>
                InvokeExcute(true, @"adb shell ""pm list features | grep host""")
                .Contains("android.hardware.usb.host"));

            labels[8].Text = $"是否支持OTG：{(hasOtgSupport ? "是" : "否")}";

            for (int i = 0; i < labels.Length; i++)
            {
                if (i != 8)
                {
                    labels[i].Text = $"{strings[i]}{GetPhoneInfo(i)}";
                }
            }
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
            // 创建并配置新窗口
            var partitionForm = new Form
            {
                Text = "选择分区",
                FormBorderStyle = FormBorderStyle.Fixed3D,
                ClientSize = new Size(300, 200),
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false
            };
            // 创建分区选择按钮
            var buttonA = CreatePartitionButton("A", new Point(50, 20), 100, 100, 30);
            var buttonB = CreatePartitionButton("B", new Point(150, 20), 100, 100, 30);
            var rebootButton = CreateRebootButton(new Point(100, 130), 100, 30);
            // 添加按钮到窗口
            partitionForm.Controls.AddRange(new Control[] { buttonA, buttonB, rebootButton });

            // 显示模态窗口
            partitionForm.ShowDialog();
        }
        private Button CreatePartitionButton(string text, Point location, int width, int height, float fontSize)
        {
            var button = new Button
            {
                Name = $"btn{text}",
                Text = text,
                Size = new Size(width, height),
                Location = location,
                Font = new Font("Microsoft YaHei UI", fontSize, FontStyle.Regular),
                TabStop = true
            };
            button.Click += but_Click;
            return button;
        }
        private Button CreateRebootButton(Point location, int width, int height)
        {
            var button = new Button
            {
                Name = "btnReboot",
                Text = "重启",
                Size = new Size(width, height),
                Location = location,
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular),
                TabStop = true
            }; 
            button.Click += but_Click;
            return button;
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
            string path = Application.StartupPath + "\\ADBDrive.exe";
            FileInfo fileInfo = new FileInfo(path);
            var segmentFileDownloader = new SegmentFileDownloader("https://los.konon.top/d/Alist/ADBDrive.exe?sign=hqgqu5AXRnGAQnXCBu9D0TapykVPn6tRE4qGvkE03dI=:0", fileInfo);
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            Form2.SystemNotify("进入后台下载！", "通知", (Icon)resources.GetObject("$this.Icon"));
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
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
        public void mess_done() { MessageBox.Show("执行成功!", "完成"); }
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
                switch(((Button)sender).Text)
                {
                    case "A":
                        if (InvokeExcute(true, string.Format("fastboot -s {0} --set-active=a", Fbdevice)).Contains("OKAY") == true)
                        {
                            mess_done();
                        }
                        else
                        {
                            MessageBox.Show("失败", "ERROR");
                        }
                        break;
                    case "B":
                        if (InvokeExcute(true, string.Format("fastboot -s {0} --set-active=b", Fbdevice)).Contains("OKAY") == true)
                        {
                            mess_done();
                        }
                        else
                        {
                            MessageBox.Show("失败", "ERROR");
                        }
                        break;
                    default:
                        if (InvokeExcute(true, string.Format("fastboot -s {0} reboot", Fbdevice)).Contains("OKAY") == true)
                        {
                            mess_done();
                        }
                        else
                        {
                            MessageBox.Show("失败", "ERROR");
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("未选择设备！");
            }
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
