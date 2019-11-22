using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using xNet;

namespace thread
{
    public partial class Form1 : Form
    {
        private List<string> SSH = new List<string>();
        private List<string> SSHlive = new List<string>();
        private string sshFile;
        public int sshdie = 0;
        public int sshlive = 0;
        public int totalSsh = 0;
        private string ffpdir = string.Concat(Environment.ExpandEnvironmentVariables("%AppData%"), "\\Mozilla\\Firefox\\Profiles\\");
        private string ffdir = "";
        private string winrarDir = "";
        private Thread mainThread;
        private Thread updatestatus;
        private Thread ffThread;
        private bool _isUpdateStatusRunning = false;
        private bool _isUpdateSSHRunning = false;
        private bool _stopFlag = false;
        private bool[] _isManualrunning;
        private bool[] ffStopFlag;
        private bool[] _isChangingSSH;
        private Thread[] mythread;
        private Thread[] myManualThread;
        private SshClient[] sshClients;
        private Random random = new Random();
        private const string userRoot = "HKEY_CURRENT_USER";
        private const string subkey = "shkey";
        private const string keyName = "HKEY_CURRENT_USER\\shkey";
        private int numOfThread = 0;
        private int[] processId;
        private Setting setting = new Setting();
        private ForwardedPortDynamic[] forwardPorts;
        private string[] freshLinkConfig;
        private DataGridViewCellStyle styleOK = new DataGridViewCellStyle();
        private DataGridViewCellStyle styleBAD = new DataGridViewCellStyle();
        private readonly static object lockObj1;
        private readonly static object lockObj2;
        private readonly static object lockObj3rowupdate;
        private readonly static object lockObj4thread;
        private readonly static object lockObjDeleteSSHDie;
        private DataGridView dataGridView1;
        private Button btnDisconnectAll;
        private Button btnConnectAll;
        private RadioButton rbtnManual;
        private RadioButton rbtnauto24;
        private Label label1;
        private NumericUpDown nmrSSHtimeout;
        private Label label2;
        private TextBox txtSSHlink;
        private Button btnLoadSSH;
        private Label label3;
        private Label lblStatus;
        private Label label4;
        private Label lblSSHtotal;
        private Label lblSSHLive;
        private Label label6;
        private Label lblSSHdie;
        private Label label7;
        private CheckBox cbLinkSSH;
        private Label label5;
        private NumericUpDown nmrThread;
        private Button btnStartFF;
        private Button btnCreateProfile;
        private DataGridViewTextBoxColumn STT;
        private DataGridViewTextBoxColumn Port;
        private DataGridViewTextBoxColumn Status;
        private DataGridViewButtonColumn Action;
        private DataGridViewButtonColumn StartFirefox;
        private DataGridViewButtonColumn CloseFF;
        private NumericUpDown nmrPort;
        private Label label8;
        private Button btnSetting;

        static Form1()
        {
            Form1.lockObj1 = new object();
            Form1.lockObj2 = new object();
            Form1.lockObj3rowupdate = new object();
            Form1.lockObj4thread = new object();
            Form1.lockObjDeleteSSHDie = new object();
        }
        public Form1()
        {
            init();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private bool _isConnected(int id)
        {
            bool flag;
            flag = (this.sshClients[id] != null ? this.sshClients[id].IsConnected : false);
            return flag;
        }
        private void btnConnectAll_Click(object sender, EventArgs e)
        {
            if (this.setting.Fresh)
            {
                string[] strArrays = File.ReadAllLines("freshLinkConfig.txt");
                if ((int)strArrays.Length >= 1)
                {
                    this.freshLinkConfig = Regex.Split(strArrays[0], "\\|");
                    if ((int)this.freshLinkConfig.Length != 2)
                    {
                        MessageBox.Show("Vui lòng check file freshLinkConfig.txt, chỉ 1 dòng\nFormat: http://google.com|keyword");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng check file freshLinkConfig.txt, chỉ 1 dòng\nFormat: http://google.com|keyword");
                    return;
                }
            }
            this._stopFlag = false;
            this.numOfThread = 0;
            this.btnConnectAll.Enabled = false;
            this.btnDisconnectAll.Enabled = false;
            this.nmrThread.Enabled = false;
            this.cbLinkSSH.Enabled = false;
            this.rbtnManual.Enabled = false;
            this.rbtnauto24.Enabled = false;
            this.dataGridView1.Rows.Clear();
            this.mainThread = new Thread(() => this.Run())
            {
                IsBackground = true
            };
            this.mainThread.Start();
        }
        private void btnCreateProfile_Click(object sender, EventArgs e)
        {
            if (this.winrarDir == "")
            {
                MessageBox.Show("Không tìm thấy winrar, vui lòng cài đặt winrar");
            }
            else if (File.Exists("Profile.rar"))
            {
                (new Thread(() => {
                    this.btnCreateProfile.Enabled = false;
                    DialogResult dialogResult = MessageBox.Show("Yes = Ghi đè profile\nNo = Không ghi đè", "Ghi đè profile", MessageBoxButtons.YesNo);
                    if (Directory.Exists(string.Concat(this.ffpdir, "ProfileCopy")))
                    {
                        Directory.Delete(string.Concat(this.ffpdir, "ProfileCopy"), true);
                    }
                    Directory.CreateDirectory(string.Concat(this.ffpdir, "ProfileCopy"));
                    this.unRar("Profile.rar", string.Concat(this.ffpdir, "ProfileCopy"), true);
                    string str = string.Concat(Environment.ExpandEnvironmentVariables("%AppData%"), "\\Mozilla\\Firefox\\profiles.ini");
                    if (File.Exists(str))
                    {
                        File.WriteAllText(str, string.Concat(new string[] { "[General]", Environment.NewLine, "StartWithLastProfile=1", Environment.NewLine, Environment.NewLine }));
                    }
                    for (int i = 0; i < Convert.ToInt32(this.nmrThread.Value); i++)
                    {
                        string str1 = string.Concat(this.ffpdir, i);
                        if ((!Directory.Exists(str1) ? true : dialogResult != DialogResult.Yes))
                        {
                            if ((!Directory.Exists(str1) ? true : dialogResult != DialogResult.No))
                            {
                                goto Label1;
                            }
                            goto Label0;
                        }
                        else
                        {
                            Directory.Delete(str1, true);
                        }
                    Label1:
                        Directory.CreateDirectory(str1);
                        this.copyFolder(string.Concat(this.ffpdir, "ProfileCopy"), str1);
                        if (File.Exists(string.Concat(str1, "\\prefs.js")))
                        {
                            File.AppendAllText(string.Concat(str1, "\\prefs.js"), string.Concat("user_pref(\"network.proxy.type\", 1);\nuser_pref(\"network.proxy.socks\", \"127.0.0.1\");\nuser_pref(\"network.proxy.socks_port\", ", 1080 + i, ");\nuser_pref(\"media.peerconnection.enabled\", false);\nuser_pref(\"network.proxy.socks_remote_dns\", true);\nuser_pref(\"privacy.donottrackheader.enabled\", true);\nuser_pref(\"browser.tabs.warnOnClose\", false);"));
                        }
                        File.AppendAllText(str, string.Concat(new object[] { "[Profile", i, "]", Environment.NewLine, "Name=", i, Environment.NewLine, "IsRelative=1", Environment.NewLine, "Path=Profiles/", i, Environment.NewLine, Environment.NewLine }));
                    }
                    Label0:
                    if (Directory.Exists(string.Concat(this.ffpdir, "ProfileCopy")))
                    {
                        Directory.Delete(string.Concat(this.ffpdir, "ProfileCopy"), true);
                    }
                    MessageBox.Show("Tạo profile thành công");
                    this.btnCreateProfile.Enabled = true;
                })).Start();
            }
            else
            {
                MessageBox.Show("Không tìm thấy file Profile.rar");
            }
        }
        private void btnDisconnectAll_Click(object sender, EventArgs e)
        {
            (new Thread(() => {
                this._stopFlag = true;
                this.btnDisconnectAll.Enabled = false;
                if (this._isUpdateStatusRunning)
                {
                    this.updatestatus.Abort();
                    this._isUpdateStatusRunning = false;
                }
                while (Array.IndexOf<bool>(this._isChangingSSH, true) != -1)
                {
                    Thread.Sleep(500);
                }
                Thread.Sleep(1000);
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    if (this.sshClients[i] != null)
                    {
                        try
                        {
                            this.sshClients[i].RemoveForwardedPort(this.forwardPorts[i]);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.sshClients[i].Disconnect();
                        }
                        catch
                        {
                        }
                        this.sshClients[i].Dispose();
                    }
                }
                this.nmrThread.Enabled = true;
                this.cbLinkSSH.Enabled = true;
                this.btnDisconnectAll.Enabled = false;
                this.btnConnectAll.Enabled = true;
                this.rbtnManual.Enabled = true;
                this.rbtnauto24.Enabled = true;
            })).Start();
        }
        private void btnLoadSSH_Click(object sender, EventArgs e)
        {
            if (this.cbLinkSSH.Checked)
            {
                MessageBox.Show("SSH import by URL!\nPlease change setting.");
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*",
                    FilterIndex = 1,
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.LoadSSH(openFileDialog.FileName);
                }
            }
        }
        private void btnSetting_Click(object sender, EventArgs e)
        {
            this.setting.ShowDialog();
        }
        private void btnStartFF_Click(object sender, EventArgs e)
        {
            if (this.btnStartFF.Text != "Start All Firefox")
            {
                try
                {
                    this.ffThread.Abort();
                }
                catch
                {
                }
                try
                {
                    Process[] processesByName = Process.GetProcessesByName("firefox");
                    for (int i = 0; i < (int)processesByName.Length; i++)
                    {
                        Process process = processesByName[i];
                        process.Kill();
                        process.Dispose();
                    }
                }
                catch
                {
                }
                this.btnStartFF.Text = "Start All Firefox";
            }
            else
            {
                this.btnStartFF.Enabled = false;
                this.ffThread = new Thread(() => this.RunFF())
                {
                    IsBackground = true
                };
                this.ffThread.Start();
            }
        }
        private bool ChangeSSH(int id, int timeOut)
        {
            string ssh;
            bool flag;
            bool flag1;
            while (!this._stopFlag)
            {
                lock (Form1.lockObj4thread)
                {
                    if (this.numOfThread < 10)
                    {
                        this.numOfThread++;
                        break;
                    }
                }
                Thread.Sleep(500);
            }
            bool flag2 = false;
            while (!this._stopFlag)
            {
                if (this.sshClients[id] != null)
                {
                    try
                    {
                        this.sshClients[id].RemoveForwardedPort(this.forwardPorts[id]);
                    }
                    catch
                    {
                    }
                    if (this.sshClients[id].IsConnected)
                    {
                        try
                        {
                            this.sshClients[id].Disconnect();
                        }
                        catch
                        {
                        }
                    }
                }
                lock (Form1.lockObj1)
                {
                    ssh = this.GetSsh();
                }
                string str = "";
                if (ssh != null)
                {
                    string[] strArrays = Regex.Split(ssh, "\\|");
                    str = strArrays[0];
                    this.updateRow(id, this.styleBAD, string.Concat("Connecting to: ", strArrays[0]), "Stop");
                    try
                    {
                        this.sshClients[id] = new SshClient(strArrays[0], strArrays[1], strArrays[2]);
                        this.sshClients[id].ConnectionInfo.Timeout = TimeSpan.FromSeconds((double)timeOut);
                        this.sshClients[id].ErrorOccurred += new EventHandler<ExceptionEventArgs>((object sender, ExceptionEventArgs e) => {
                        });
                        this.sshClients[id].KeepAliveInterval = TimeSpan.FromSeconds(60);
                        this.sshClients[id].Connect();
                        if (this.sshClients[id].IsConnected)
                        {
                            if (!this._stopFlag)
                            {
                                try
                                {
                                    this.sshClients[id].AddForwardedPort(this.forwardPorts[id]);
                                    this.forwardPorts[id].Start();
                                    if (this.forwardPorts[id].IsStarted)
                                    {
                                        string str1 = this.FreshAndWhoer(1080 + id);
                                        if (str1 != "ERROR-FRESH")
                                        {
                                            if (str1 == "OK")
                                            {
                                                flag1 = true;
                                            }
                                            else
                                            {
                                                flag1 = (str1 != "ERROR" ? false : !this.setting.WhoerError);
                                            }
                                            if (flag1)
                                            {
                                                flag2 = true;
                                            }
                                        }
                                        else
                                        {
                                            flag2 = false;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                if (flag2)
                {
                    this.sshlive++;
                    this.updateRow(id, this.styleOK, string.Concat("Connected : ", str), "Change");
                    if ((!this.rbtnManual.Checked ? false : this.setting.DeleteSSHdie))
                    {
                        lock (Form1.lockObjDeleteSSHDie)
                        {
                            this.SSHlive.Add(ssh);
                            List<string> strs = new List<string>();
                            strs.AddRange(this.SSHlive);
                            strs.AddRange(this.SSH);
                            File.WriteAllLines(this.sshFile, strs);
                        }
                    }
                    break;
                }
                else if (ssh != null)
                {
                    this.sshdie++;
                }
                else
                {
                    break;
                }
            }
            lock (Form1.lockObj4thread)
            {
                this.numOfThread--;
            }
            flag = flag2;
            return flag;
        }
        private void copyFolder(string fromFolder, string toFolder)
        {
            this.RunCMD(string.Concat(new string[] { "Xcopy /E /I /c /y \"", fromFolder, "\" \"", toFolder, "\"" }));
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex != 3 ? false : e.RowIndex >= 0))
            {
                if ((!this.rbtnauto24.Checked ? true : this._isConnected(e.RowIndex)))
                {
                    if (!this._isManualrunning[e.RowIndex])
                    {
                        this.myManualThread[e.RowIndex] = new Thread(() => this.ManualSSH(e.RowIndex, e.ColumnIndex));
                        this.myManualThread[e.RowIndex].IsBackground = true;
                        this.myManualThread[e.RowIndex].Start();
                    }
                    else
                    {
                        this.myManualThread[e.RowIndex].Abort();
                        this._isManualrunning[e.RowIndex] = false;
                        (new Thread(() => {
                            try
                            {
                                this.sshClients[e.RowIndex].RemoveForwardedPort(this.forwardPorts[e.RowIndex]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                this.sshClients[e.RowIndex].Disconnect();
                            }
                            catch
                            {
                                MessageBox.Show("Lỗi không thể disconnect ssh");
                            }
                            this.updateRow(e.RowIndex, this.styleBAD, "Stopped", "Change");
                            this._isChangingSSH[e.RowIndex] = false;
                        })).Start();
                    }
                }
            }
            else if ((e.ColumnIndex != 4 ? false : e.RowIndex >= 0))
            {
                (new Thread(() => this.Firefox(e.RowIndex))).Start();
            }
            else if ((e.ColumnIndex != 5 ? false : e.RowIndex >= 0))
            {
                this.ffStopFlag[e.RowIndex] = true;
            }
        }
        private void DownloadFile(string link, string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(link, filepath);
            }
        }
        private bool downloadSSH()
        {
            bool flag;
            int num = 0;
            while (true)
            {
                if (this._stopFlag)
                {
                    flag = true;
                    return flag;
                }
                if (num != 5)
                {
                    try
                    {
                        this.DownloadFile(this.txtSSHlink.Text, "SSH_DOWNLOAD.txt");
                    }
                    catch
                    {
                        num++;
                        Thread.Sleep(5000);
                        continue;
                    }
                    if (!this.setting.Duplicate)
                    {
                        this.LoadSSH("SSH_DOWNLOAD.txt");
                        flag = true;
                        return flag;
                    }
                    else
                    {
                        string str = "SSH_OLD.txt";
                        if (this.txtSSHlink.Text.Contains("sellssh247.com"))
                        {
                            Match match = Regex.Match(this.txtSSHlink.Text, "code=([a-zA-Z]{2})");
                            if (match.Success)
                            {
                                str = string.Concat(match.Groups[1].Value, ".txt");
                            }
                        }
                        if (!File.Exists(str))
                        {
                            string[] strArrays = File.ReadAllLines("SSH_DOWNLOAD.txt");
                            if ((int)strArrays.Length >= 2)
                            {
                                File.WriteAllLines(str, strArrays);
                                this.LoadSSH("SSH_DOWNLOAD.txt");
                                break;
                            }
                            else
                            {
                                base.Invoke(new MethodInvoker(() => this.lblStatus.Text = "SSH new = 0, sleep 10min"));
                                Thread.Sleep(600000);
                            }
                        }
                        else
                        {
                            string[] strArrays1 = File.ReadAllLines(str);
                            List<string> strs = new List<string>();
                            string[] strArrays2 = File.ReadAllLines("SSH_DOWNLOAD.txt");
                            for (int i = 0; i < (int)strArrays2.Length; i++)
                            {
                                string[] strArrays3 = Regex.Split(strArrays2[i], "\\|");
                                if ((int)strArrays3.Length >= 3)
                                {
                                    string str1 = string.Concat(new string[] { strArrays3[0].Trim(), "|", strArrays3[1].Trim(), "|", strArrays3[2].Trim() });
                                    if (strArrays1.Contains<string>(str1))
                                    {
                                        goto Label3;
                                    }
                                    strs.Add(str1);
                                }
                            }
                            Label3:
                            if (strs.Count != 0)
                            {
                                File.AppendAllLines(str, strs);
                                this.SSH.AddRange(strs);
                                break;
                            }
                            else
                            {
                                base.Invoke(new MethodInvoker(() => this.lblStatus.Text = "SSH new = 0, sleep 10min"));
                                int num1 = 0;
                                while (num1 < 600)
                                {
                                    if (!this._stopFlag)
                                    {
                                        Thread.Sleep(num1 * 1000);
                                        num1++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    base.Invoke(new MethodInvoker(() => this.lblStatus.Text = "ERROR DOWNLOAD FILE"));
                    flag = false;
                    return flag;
                }
            }
            flag = true;
            return flag;
        }
        private void Firefox(int threadId)
        {
            

            if (Directory.Exists(string.Concat(this.ffpdir, threadId)))
            {
                // process param
                var cService = FirefoxDriverService.CreateDefaultService();
                cService.HideCommandPromptWindow = true;
                // profile param
                var profileManager = new FirefoxProfileManager();
                FirefoxProfile profile = profileManager.GetProfile(threadId.ToString());
                // firefox driver options
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArguments("disable-infobars");
                firefoxOptions.Profile = profile;
                // start firefox
                try
                {
                    IWebDriver Driver = new FirefoxDriver(cService,firefoxOptions);
                    Driver.Navigate().GoToUrl("https://whoer.net");
                    this.processId[threadId] = cService.ProcessId;
                }
                catch (Exception e) { Console.WriteLine(e); }
                // process manage
                Process process = Process.GetProcessById(this.processId[threadId]);
                this.ffStopFlag[threadId] = false;
                while (true)
                {
                    if (process.HasExited)
                    {
                        break;
                    }
                    else if ((!this.ffStopFlag[threadId] ? false : !process.HasExited))
                    {
                        process.CloseMainWindow();
                        Thread.Sleep(3000);
                        break;
                    }
                    else if (!this.ffStopFlag[threadId])
                    {
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        break;
                    }
                }
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            else
            {
                MessageBox.Show(string.Concat("Profile ", threadId, " not found."));
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._isUpdateStatusRunning)
            {
                this.updatestatus.Abort();
                this._isUpdateStatusRunning = false;
            }
            if (this.rbtnauto24.Checked)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.mythread[i].Abort();
                }
            }
            if (File.Exists("Setting.xml"))
            {
                XDocument text = XDocument.Load("Setting.xml");
                XElement str = text.Root.Element("Setting").Element("Proxy");
                bool proxy = this.setting.Proxy;
                str.Value = proxy.ToString();
                XElement xElement = text.Root.Element("Setting").Element("Anonymizer");
                proxy = this.setting.Anonymizer;
                xElement.Value = proxy.ToString();
                XElement str1 = text.Root.Element("Setting").Element("Blacklist");
                proxy = this.setting.Blacklist;
                str1.Value = proxy.ToString();
                XElement xElement1 = text.Root.Element("Setting").Element("WhoerError");
                proxy = this.setting.WhoerError;
                xElement1.Value = proxy.ToString();
                XElement str2 = text.Root.Element("Setting").Element("Duplicate");
                proxy = this.setting.Duplicate;
                str2.Value = proxy.ToString();
                XElement xElement2 = text.Root.Element("Setting").Element("Fresh");
                proxy = this.setting.Fresh;
                xElement2.Value = proxy.ToString();
                XElement str3 = text.Root.Element("Setting").Element("Unused");
                proxy = this.setting.Unused;
                str3.Value = proxy.ToString();
                XElement xElement3 = text.Root.Element("Setting").Element("DeleteSSHdie");
                proxy = this.setting.DeleteSSHdie;
                xElement3.Value = proxy.ToString();
                XElement str4 = text.Root.Element("Setting").Element("RandomSSH");
                proxy = this.setting.RandomSSH;
                str4.Value = proxy.ToString();
                XElement xElement4 = text.Root.Element("Setting").Element("nmrSSHtimeout");
                decimal value = this.nmrSSHtimeout.Value;
                xElement4.Value = value.ToString();
                text.Root.Element("Setting").Element("txtSSHlink").Value = this.txtSSHlink.Text;
                XElement str5 = text.Root.Element("Setting").Element("cbLinkSSH");
                proxy = this.cbLinkSSH.Checked;
                str5.Value = proxy.ToString();
                XElement xElement5 = text.Root.Element("Setting").Element("nmrThread");
                value = this.nmrThread.Value;
                xElement5.Value = value.ToString();
                XElement str6 = text.Root.Element("Setting").Element("FFProfileScan");
                proxy = this.setting.FFProfileScan;
                str6.Value = proxy.ToString();
                XElement xElement6 = text.Root.Element("Setting").Element("FFProfileThreadId");
                proxy = this.setting.FFProfileThreadId;
                xElement6.Value = proxy.ToString();
                text.Save("Setting.xml");
            }
            else
            {
                (new XDocument(new object[] { new XElement("Root", new XElement("Setting", new object[] { new XElement("Proxy", (object)this.setting.Proxy), new XElement("Anonymizer", (object)this.setting.Anonymizer), new XElement("Blacklist", (object)this.setting.Blacklist), new XElement("WhoerError", (object)this.setting.WhoerError), new XElement("Duplicate", (object)this.setting.Duplicate), new XElement("Fresh", (object)this.setting.Fresh), new XElement("Unused", (object)this.setting.Unused), new XElement("DeleteSSHdie", (object)this.setting.DeleteSSHdie), new XElement("RandomSSH", (object)this.setting.RandomSSH), new XElement("nmrSSHtimeout", (object)this.nmrSSHtimeout.Value), new XElement("txtSSHlink", this.txtSSHlink.Text), new XElement("cbLinkSSH", (object)this.cbLinkSSH.Checked), new XElement("nmrThread", (object)this.nmrThread.Value), new XElement("FFProfileScan", (object)this.setting.FFProfileScan), new XElement("FFProfileThreadId", (object)this.setting.FFProfileThreadId) })) })).Save("Setting.xml");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if ((int)Process.GetProcessesByName("SSH CHANGER MultiPort").Length > 1)
            {
                Application.Exit();
            }
            if ((string)Registry.GetValue("HKEY_CURRENT_USER\\shkey", "key", "NULL") != "OK")
            {
                Form2 form2 = new Form2();
                form2.ShowDialog();
                if (form2.key != "dinhtai92dn")
                {
                    MessageBox.Show("Sai key");
                    Application.Exit();
                }
                else
                {
                    Registry.SetValue("HKEY_CURRENT_USER\\shkey", "key", "OK", RegistryValueKind.String);
                }
            }
            if (File.Exists(string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "\\Mozilla Firefox\\firefox.exe")))
            {
                this.ffdir = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "\\Mozilla Firefox\\firefox.exe");
            }
            else if (File.Exists(string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "\\Mozilla Firefox\\firefox.exe")))
            {
                this.ffdir = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "\\Mozilla Firefox\\firefox.exe");
            }
            else if (File.Exists("C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe"))
            {
                this.ffdir = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            }
            else if (File.Exists("C:\\Program Files\\Mozilla Firefox\\firefox.exe"))
            {
                this.ffdir = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
            }
            else if (!File.Exists("firefoxdir.ini"))
            {
                MessageBox.Show("Không tìm thấy firefox.exe");
            }
            else
            {
                this.ffdir = File.ReadAllText("firefoxdir.ini");
                MessageBox.Show(string.Concat("Không tìm thấy firefox", Environment.NewLine, "Lấy đường dẫn firefox.exe trong file firefoxdir.ini"));
            }
            if (File.Exists("C:\\Program Files\\WinRAR\\WinRAR.exe"))
            {
                this.winrarDir = "C:\\Program Files\\WinRAR\\WinRAR.exe";
            }
            else if (!File.Exists("C:\\Program Files (x86)\\WinRAR\\WinRAR.exe"))
            {
                MessageBox.Show("Không tìm thấy winrar");
            }
            else
            {
                this.winrarDir = "C:\\Program Files (x86)\\WinRAR\\WinRAR.exe";
            }
            this.styleOK.BackColor = Color.Green;
            this.styleBAD.BackColor = Color.Yellow;
            if (File.Exists("Setting.xml"))
            {
                XDocument xDocument = XDocument.Load("Setting.xml");
                this.setting.Proxy = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("Proxy").Value);
                this.setting.Anonymizer = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("Anonymizer").Value);
                this.setting.Blacklist = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("Blacklist").Value);
                this.setting.WhoerError = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("WhoerError").Value);
                this.setting.Duplicate = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("Duplicate").Value);
                this.setting.Fresh = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("Fresh").Value);
                this.setting.Unused = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("Unused").Value);
                this.setting.DeleteSSHdie = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("DeleteSSHdie").Value);
                this.setting.RandomSSH = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("RandomSSH").Value);
                this.nmrSSHtimeout.Value = Convert.ToInt32(xDocument.Root.Element("Setting").Element("nmrSSHtimeout").Value);
                this.txtSSHlink.Text = xDocument.Root.Element("Setting").Element("txtSSHlink").Value;
                this.cbLinkSSH.Checked = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("cbLinkSSH").Value);
                this.nmrThread.Value = Convert.ToInt32(xDocument.Root.Element("Setting").Element("nmrThread").Value);
                this.setting.FFProfileScan = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("FFProfileScan").Value);
                this.setting.FFProfileThreadId = Convert.ToBoolean(xDocument.Root.Element("Setting").Element("FFProfileThreadId").Value);
            }
        }
        private string FreshAndWhoer(int port)
        {
            Match match;
            string str;
            if ((this.setting.Fresh || this.setting.Proxy || this.setting.Anonymizer ? true : this.setting.Blacklist))
            {
                using (HttpRequest httpRequest = new HttpRequest())
                {
                    httpRequest.Proxy = new Socks5ProxyClient("127.0.0.1", port);
                    httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36";
                    string str1 = "";
                    if (this.setting.Fresh)
                    {
                        try
                        {
                            str1 = httpRequest.Get(this.freshLinkConfig[0].Trim(), null).ToString();
                            if (!str1.Contains(this.freshLinkConfig[1].Trim()))
                            {
                                str = "UNFRESH";
                                return str;
                            }
                        }
                        catch
                        {
                            str = "ERROR-FRESH";
                            return str;
                        }
                    }
                    if ((this.setting.Proxy || this.setting.Anonymizer ? true : this.setting.Blacklist))
                    {
                        try
                        {
                            str1 = httpRequest.Get("http://whoer.net", null).ToString();
                        }
                        catch
                        {
                            str = "ERROR";
                            return str;
                        }
                    }
                    if (this.setting.Proxy)
                    {
                        match = Regex.Match(str1, "proxy : \"([^\"]+)", RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                            str = "ERROR-CHECK";
                            return str;
                        }
                        else if (match.Groups[1].Value.ToString() != "0")
                        {
                            str = "proxy";
                            return str;
                        }
                    }
                    if (this.setting.Anonymizer)
                    {
                        match = Regex.Match(str1, "anonymizer : ([^,]+)", RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                            str = "ERROR-CHECK";
                            return str;
                        }
                        else if (match.Groups[1].Value.ToString() != "0")
                        {
                            str = "anonymizer";
                            return str;
                        }
                    }
                    if (this.setting.Blacklist)
                    {
                        match = Regex.Match(str1, "dsbl : ([^\\n]+)", RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                            str = "ERROR-CHECK";
                            return str;
                        }
                        else if (match.Groups[1].Value.ToString() != "0")
                        {
                            str = "dsbl";
                            return str;
                        }
                    }
                    try
                    {
                        httpRequest.Close();
                    }
                    catch
                    {
                        str = "ERROR";
                        return str;
                    }
                }
                str = "OK";
                return str;
            }
            else
            {
                str = "OK";
                return str;
            }
            return str;
        }
        private string GetSsh()
        {
            string str;
            if (this.SSH.Count != 0)
            {
                int num = 0;
                if (this.setting.RandomSSH)
                {
                    num = this.random.Next(0, this.SSH.Count - 1);
                }
                string item = this.SSH[num];
                this.SSH.RemoveAt(num);
                if (this.setting.Unused)
                {
                    File.WriteAllLines("SSH_UnUsed.txt", this.SSH);
                }
                str = item;
            }
            else
            {
                str = null;
            }
            return str;
        }
        private void init()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.dataGridView1 = new DataGridView();
            this.STT = new DataGridViewTextBoxColumn();
            this.Port = new DataGridViewTextBoxColumn();
            this.Status = new DataGridViewTextBoxColumn();
            this.Action = new DataGridViewButtonColumn();
            this.StartFirefox = new DataGridViewButtonColumn();
            this.CloseFF = new DataGridViewButtonColumn();
            this.btnDisconnectAll = new Button();
            this.btnConnectAll = new Button();
            this.rbtnManual = new RadioButton();
            this.rbtnauto24 = new RadioButton();
            this.label1 = new Label();
            this.nmrSSHtimeout = new NumericUpDown();
            this.label2 = new Label();
            this.txtSSHlink = new TextBox();
            this.btnLoadSSH = new Button();
            this.label3 = new Label();
            this.lblStatus = new Label();
            this.label4 = new Label();
            this.lblSSHtotal = new Label();
            this.lblSSHLive = new Label();
            this.label6 = new Label();
            this.lblSSHdie = new Label();
            this.label7 = new Label();
            this.cbLinkSSH = new CheckBox();
            this.label5 = new Label();
            this.nmrThread = new NumericUpDown();
            this.btnStartFF = new Button();
            this.btnCreateProfile = new Button();
            this.nmrPort = new NumericUpDown();
            this.label8 = new Label();
            this.btnSetting = new Button();
            ((ISupportInitialize)this.dataGridView1).BeginInit();
            ((ISupportInitialize)this.nmrSSHtimeout).BeginInit();
            ((ISupportInitialize)this.nmrThread).BeginInit();
            ((ISupportInitialize)this.nmrPort).BeginInit();
            base.SuspendLayout();
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle.BackColor = SystemColors.Control;
            dataGridViewCellStyle.Font = new Font("Microsoft Sans Serif", 7.8f, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new DataGridViewColumn[] { this.STT, this.Port, this.Status, this.Action, this.StartFirefox, this.CloseFF });
            this.dataGridView1.Location = new Point(13, 187);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new Size(806, 370);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.STT.HeaderText = "#";
            this.STT.Name = "STT";
            this.STT.ReadOnly = true;
            this.STT.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.STT.Width = 37;
            this.Port.HeaderText = "Cổng";
            this.Port.Name = "Port";
            this.Port.ReadOnly = true;
            this.Port.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.Port.Width = 43;
            this.Status.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Status.HeaderText = "Trạng Thái";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.Action.HeaderText = "Hành Động";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            this.Action.Resizable = DataGridViewTriState.True;
            this.Action.Width = 80;
            this.StartFirefox.HeaderText = "Mở Firefox";
            this.StartFirefox.Name = "StartFirefox";
            this.StartFirefox.ReadOnly = true;
            this.StartFirefox.Width = 80;
            this.CloseFF.HeaderText = "Đóng Firefox";
            this.CloseFF.Name = "CloseFF";
            this.CloseFF.ReadOnly = true;
            this.CloseFF.Width = 80;
            this.btnDisconnectAll.Enabled = false;
            this.btnDisconnectAll.Location = new Point(696, 92);
            this.btnDisconnectAll.Name = "btnDisconnectAll";
            this.btnDisconnectAll.Size = new Size(119, 30);
            this.btnDisconnectAll.TabIndex = 1;
            this.btnDisconnectAll.Text = "Disconnect All";
            this.btnDisconnectAll.UseVisualStyleBackColor = true;
            this.btnDisconnectAll.Click += new EventHandler(this.btnDisconnectAll_Click);
            this.btnConnectAll.Location = new Point(577, 92);
            this.btnConnectAll.Name = "btnConnectAll";
            this.btnConnectAll.Size = new Size(97, 30);
            this.btnConnectAll.TabIndex = 2;
            this.btnConnectAll.Text = "Start";
            this.btnConnectAll.UseVisualStyleBackColor = true;
            this.btnConnectAll.Click += new EventHandler(this.btnConnectAll_Click);
            this.rbtnManual.AutoSize = true;
            this.rbtnManual.Checked = true;
            this.rbtnManual.Location = new Point(13, 13);
            this.rbtnManual.Name = "rbtnManual";
            this.rbtnManual.Size = new Size(75, 21);
            this.rbtnManual.TabIndex = 3;
            this.rbtnManual.TabStop = true;
            this.rbtnManual.Text = "Mặc Định";
            this.rbtnManual.UseVisualStyleBackColor = true;
            this.rbtnauto24.AutoSize = true;
            this.rbtnauto24.Location = new Point(110, 13);
            this.rbtnauto24.Name = "rbtnauto24";
            this.rbtnauto24.Size = new Size(98, 21);
            this.rbtnauto24.TabIndex = 4;
            this.rbtnauto24.Text = "Tự Động";
            this.rbtnauto24.UseVisualStyleBackColor = true;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(245, 15);
            this.label1.Name = "label1";
            this.label1.Size = new Size(116, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "SSH Timeout (s):";
            this.nmrSSHtimeout.Location = new Point(368, 15);
            this.nmrSSHtimeout.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.nmrSSHtimeout.Name = "nmrSSHtimeout";
            this.nmrSSHtimeout.Size = new Size(71, 22);
            this.nmrSSHtimeout.TabIndex = 6;
            this.nmrSSHtimeout.Value = new decimal(new int[] { 9, 0, 0, 0 });
            this.label2.AutoSize = true;
            this.label2.Location = new Point(472, 19);
            this.label2.Name = "label2";
            this.label2.Size = new Size(66, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "LinkSSH:";
            this.txtSSHlink.Location = new Point(545, 19);
            this.txtSSHlink.Name = "txtSSHlink";
            this.txtSSHlink.Size = new Size(270, 22);
            this.txtSSHlink.TabIndex = 8;
            this.btnLoadSSH.Location = new Point(13, 55);
            this.btnLoadSSH.Name = "btnLoadSSH";
            this.btnLoadSSH.Size = new Size(87, 30);
            this.btnLoadSSH.TabIndex = 9;
            this.btnLoadSSH.Text = "Chọn SSH";
            this.btnLoadSSH.UseVisualStyleBackColor = true;
            this.btnLoadSSH.Click += new EventHandler(this.btnLoadSSH_Click);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(13, 104);
            this.label3.Name = "label3";
            this.label3.Size = new Size(52, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Status:";
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(72, 104);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(20, 17);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "...";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(103, 60);
            this.label4.Name = "label4";
            this.label4.Size = new Size(44, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "SHH :";
            this.lblSSHtotal.AutoSize = true;
            this.lblSSHtotal.Location = new Point(149, 60);
            this.lblSSHtotal.Name = "lblSSHtotal";
            this.lblSSHtotal.Size = new Size(20, 17);
            this.lblSSHtotal.TabIndex = 13;
            this.lblSSHtotal.Text = "...";
            this.lblSSHLive.AutoSize = true;
            this.lblSSHLive.Location = new Point(259, 61);
            this.lblSSHLive.Name = "lblSSHLive";
            this.lblSSHLive.Size = new Size(20, 17);
            this.lblSSHLive.TabIndex = 15;
            this.lblSSHLive.Text = "...";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(218, 61);
            this.label6.Name = "label6";
            this.label6.Size = new Size(38, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "Live:";
            this.lblSSHdie.AutoSize = true;
            this.lblSSHdie.Location = new Point(367, 61);
            this.lblSSHdie.Name = "lblSSHdie";
            this.lblSSHdie.Size = new Size(20, 17);
            this.lblSSHdie.TabIndex = 17;
            this.lblSSHdie.Text = "...";
            this.label7.AutoSize = true;
            this.label7.Location = new Point(331, 61);
            this.label7.Name = "label7";
            this.label7.Size = new Size(33, 17);
            this.label7.TabIndex = 16;
            this.label7.Text = "Die:";
            this.cbLinkSSH.AutoSize = true;
            this.cbLinkSSH.Location = new Point(451, 57);
            this.cbLinkSSH.Name = "cbLinkSSH";
            this.cbLinkSSH.Size = new Size(186, 21);
            this.cbLinkSSH.TabIndex = 18;
            this.cbLinkSSH.Text = "Download SSH";
            this.cbLinkSSH.UseVisualStyleBackColor = true;
            this.label5.AutoSize = true;
            this.label5.Location = new Point(670, 58);
            this.label5.Name = "label5";
            this.label5.Size = new Size(58, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "Số Luồng:";
            this.nmrThread.Location = new Point(743, 56);
            this.nmrThread.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.nmrThread.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nmrThread.Name = "nmrThread";
            this.nmrThread.Size = new Size(63, 22);
            this.nmrThread.TabIndex = 20;
            this.nmrThread.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.btnStartFF.Location = new Point(435, 92);
            this.btnStartFF.Name = "btnStartFF";
            this.btnStartFF.Size = new Size(116, 30);
            this.btnStartFF.TabIndex = 21;
            this.btnStartFF.Text = "Mở Tất Cả";
            this.btnStartFF.UseVisualStyleBackColor = true;
            this.btnStartFF.Click += new EventHandler(this.btnStartFF_Click);
            this.btnCreateProfile.Location = new Point(298, 92);
            this.btnCreateProfile.Name = "btnCreateProfile";
            this.btnCreateProfile.Size = new Size(116, 30);
            this.btnCreateProfile.TabIndex = 22;
            this.btnCreateProfile.Text = "Tạo Profile";
            this.btnCreateProfile.UseVisualStyleBackColor = true;
            this.btnCreateProfile.Click += new EventHandler(this.btnCreateProfile_Click);
            this.nmrPort.Location = new Point(367, 145);
            this.nmrPort.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.nmrPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nmrPort.Name = "nmrPort";
            this.nmrPort.Size = new Size(102, 22);
            this.nmrPort.TabIndex = 25;
            this.nmrPort.Value = new decimal(new int[] { 1080, 0, 0, 0 });
            this.nmrPort.ValueChanged += new EventHandler(this.nmrPort_ValueChanged);
            this.nmrPort.KeyUp += new KeyEventHandler(this.nmrPort_KeyUp);
            this.label8.AutoSize = true;
            this.label8.Location = new Point(297, 147);
            this.label8.Name = "label8";
            this.label8.Size = new Size(64, 17);
            this.label8.TabIndex = 26;
            this.label8.Text = "Find port";
            this.btnSetting.Location = new Point(13, 141);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new Size(75, 28);
            this.btnSetting.TabIndex = 27;
            this.btnSetting.Text = "Cài Đặt";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new EventHandler(this.btnSetting_Click);
            base.AutoScaleDimensions = new SizeF(8f, 16f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(830, 574);
            base.Controls.Add(this.btnSetting);
            base.Controls.Add(this.label8);
            base.Controls.Add(this.nmrPort);
            base.Controls.Add(this.btnCreateProfile);
            base.Controls.Add(this.btnStartFF);
            base.Controls.Add(this.nmrThread);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.cbLinkSSH);
            base.Controls.Add(this.lblSSHdie);
            base.Controls.Add(this.label7);
            base.Controls.Add(this.lblSSHLive);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.lblSSHtotal);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.lblStatus);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.btnLoadSSH);
            base.Controls.Add(this.txtSSHlink);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.nmrSSHtimeout);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.rbtnauto24);
            base.Controls.Add(this.rbtnManual);
            base.Controls.Add(this.btnConnectAll);
            base.Controls.Add(this.btnDisconnectAll);
            base.Controls.Add(this.dataGridView1);
            base.Name = "Dashboard";
            this.Text = "Tool Fake Made by Nguyễn Thành Đạt";
            base.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            base.Load += new EventHandler(this.Form1_Load);
            ((ISupportInitialize)this.dataGridView1).EndInit();
            ((ISupportInitialize)this.nmrSSHtimeout).EndInit();
            ((ISupportInitialize)this.nmrThread).EndInit();
            ((ISupportInitialize)this.nmrPort).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
        private bool isSshLive(int id)
        {
            bool flag;
            HttpRequest httpRequest = new HttpRequest()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.119 Safari/537.36",
                Proxy = new Socks5ProxyClient("127.0.0.1", 1080 + id),
                ConnectTimeout = 15000,
                IgnoreProtocolErrors = true
            };
            try
            {
                string str = httpRequest.Get(this.freshLinkConfig[0].Trim(), null).ToString();
                httpRequest.Close();
                flag = (!str.Contains(this.freshLinkConfig[1].Trim()) ? false : true);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        private void LoadSSH(string _sshpath)
        {
            if (!File.Exists(_sshpath))
            {
                MessageBox.Show("Please check file path", "File not fund", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.sshFile = _sshpath;
                List<string> strs = new List<string>();
                this.SSH.Clear();
                this.SSHlive.Clear();
                this.sshdie = 0;
                this.sshlive = 0;
                string[] strArrays = File.ReadAllLines(_sshpath);
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    try
                    {
                        if (str.Trim().Length != 0)
                        {
                            string[] strArrays1 = Regex.Split(str, "\\|");
                            if ((int)strArrays1.Length < 3)
                            {
                                goto Label0;
                            }
                            else if ((!Regex.IsMatch(strArrays1[0].Trim(), "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$") || !(strArrays1[1].Trim() != "") ? false : strArrays1[2].Trim() != ""))
                            {
                                strs.Add(string.Concat(new string[] { strArrays1[0].Trim(), "|", strArrays1[1].Trim(), "|", strArrays1[2].Trim() }));
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                Label0:
                    this.SSH = strs.Distinct<string>().ToList<string>();
                    this.totalSsh = this.SSH.Count;
                    if (this.totalSsh != 0)
                    {
                        base.Invoke(new MethodInvoker(() => {
                            this.lblSSHtotal.Text = this.totalSsh.ToString();
                            this.lblSSHLive.Text = this.sshlive.ToString();
                            this.lblSSHdie.Text = this.sshdie.ToString();
                        }));
                    }
                    else
                    {
                        MessageBox.Show("SSH file no data!\nPlease check ssh file or ssh link.", "ERROR SSH", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    }
            }
        }
        private void ManualSSH(int rowindex, int cellindex)
        {
            this._isManualrunning[rowindex] = true;
            this._isChangingSSH[rowindex] = true;
            if ((cellindex != 3 ? false : rowindex >= 0))
            {
                this.updateRow(rowindex, this.styleBAD, "Changing", "Stop");
                int num = Convert.ToInt32(this.nmrSSHtimeout.Value);
                while (!this._stopFlag)
                {
                    bool flag = this.ChangeSSH(rowindex, num);
                    if (this._stopFlag)
                    {
                        break;
                    }
                    else if (flag)
                    {
                        break;
                    }
                    else if ((!this.cbLinkSSH.Checked ? false : !this._isUpdateSSHRunning))
                    {
                        lock (Form1.lockObj2)
                        {
                            this._isUpdateSSHRunning = true;
                            this.updateRow(rowindex, this.styleBAD, "Updating ssh", "Stop");
                            bool flag1 = this.downloadSSH();
                            this._isUpdateSSHRunning = false;
                            if (!flag1)
                            {
                                this.updateRow(rowindex, this.styleBAD, "ERROR SSH DOWNLOAD", "Change");
                                break;
                            }
                        }
                    }
                    else if ((!this.cbLinkSSH.Checked ? true : !this._isUpdateSSHRunning))
                    {
                        this.updateRow(rowindex, this.styleBAD, "Stop, SSH out of stock.", "Change");
                        break;
                    }
                    else
                    {
                        this.updateRow(rowindex, this.styleBAD, "Wait for update ssh", "Stop");
                        while (true)
                        {
                            if ((this._stopFlag ? true : !this._isUpdateSSHRunning))
                            {
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            if (this._stopFlag)
            {
                this.updateRow(rowindex, this.styleBAD, "Stopped", "Change");
            }
            this._isManualrunning[rowindex] = false;
            this._isChangingSSH[rowindex] = false;
        }
        private void nmrPort_KeyUp(object sender, KeyEventArgs e)
        {
            int num = Convert.ToInt32(this.nmrPort.Value - new decimal(1080));
            if ((num < 0 ? false : num < this.dataGridView1.Rows.Count))
            {
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[num].Cells[2];
            }
        }
        private void nmrPort_ValueChanged(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(this.nmrPort.Value - new decimal(1080));
            if ((num < 0 ? false : num < this.dataGridView1.Rows.Count))
            {
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[num].Cells[2];
            }
        }
        public bool PingHost(string _HostURI, int _PortNumber)
        {
            bool flag;
            try
            {
                using (TcpClient tcpClient = new TcpClient(_HostURI, _PortNumber))
                {
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        private string RandomString(int len)
        {
            string str = "abcdefghijklmnopqrstuvwxyz";
            char[] chrArray = new char[len];
            for (int i = 0; i < len; i++)
            {
                chrArray[i] = str[this.random.Next(str.Length)];
            }
            return new string(chrArray);
        }
        private void Run()
        {
            int num = Convert.ToInt32(this.nmrThread.Value);
            this.mythread = new Thread[num];
            this.myManualThread = new Thread[num];
            this.sshClients = new SshClient[num];
            this.ffStopFlag = new bool[num];
            this.forwardPorts = new ForwardedPortDynamic[num];
            this._isChangingSSH = new bool[num];
            this._isManualrunning = new bool[num];
            this.processId = new int[num];
            for (int i = 0; i < num; i++)
            {
                this.forwardPorts[i] = new ForwardedPortDynamic("127.0.0.1", Convert.ToUInt32(1080 + i));
                this.forwardPorts[i].Exception += new EventHandler<ExceptionEventArgs>((object sender, ExceptionEventArgs e) => {
                });
                string[] str = new string[] { i.ToString(), null, null, null, null, null };
                str[1] = (1080 + i).ToString();
                str[2] = "";
                str[3] = "Đổi SSH";
                str[4] = "Mở Firefox";
                str[5] = "Đóng Firefox";
                string[] strArrays = str;
                base.Invoke(new MethodInvoker(() => this.dataGridView1.Rows.Add(strArrays)));
            }
            if (!this._isUpdateStatusRunning)
            {
                this.updatestatus = new Thread(() => this.ShowStatus())
                {
                    IsBackground = true
                };
                this.updatestatus.Start();
                this._isUpdateStatusRunning = true;
            }
            Thread.Sleep(1000);
            if (this.rbtnauto24.Checked)
            {
                try
                {
                    for (int j = 0; j < num; j++)
                    {
                        this.mythread[j] = new Thread(() => this.StartThread(j));
                        this.mythread[j].IsBackground = true;
                        this.mythread[j].Start();
                        Thread.Sleep(100);
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    base.Invoke(new MethodInvoker(() => this.lblStatus.Text = exception.ToString()));
                }
            }
            this.btnDisconnectAll.Enabled = true;
        }
        private string RunCMD(string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = string.Concat("/k ", cmd);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            string end = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return (!string.IsNullOrEmpty(end) ? end : "");
        }
        private void RunFF()
        {
            this.btnStartFF.Enabled = false;
            if (this.setting.FFProfileThreadId)
            {
                int num = Convert.ToInt32(this.dataGridView1.Rows.Count);
                try
                {
                    for (int i = 0; i < num; i++)
                    {
                        (new Thread(() => this.Firefox(i))).Start();
                        Thread.Sleep(2000);
                    }
                }
                catch
                {
                    MessageBox.Show("Lỗi open Firefox");
                }
            }
            this.btnStartFF.Enabled = true;
            this.btnStartFF.Text = "Close All Firefox";
        }
        private void ShowStatus()
        {
            while (true)
            {
                base.Invoke(new MethodInvoker(() => {
                    this.lblSSHLive.Text = this.sshlive.ToString();
                    this.lblSSHdie.Text = this.sshdie.ToString();
                }));
                Thread.Sleep(1000);
            }
        }
        private void StartThread(int threadId)
        {
            bool flag;
            int num = Convert.ToInt32(this.nmrSSHtimeout.Value);
            this._isChangingSSH[threadId] = true;
            while (!this._stopFlag)
            {
                bool flag1 = this.ChangeSSH(threadId, num);
                if (this._stopFlag)
                {
                    break;
                }
                else if (flag1)
                {
                    Thread.Sleep(1000);
                    while (true)
                    {
                        if (this._stopFlag)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (this._isManualrunning[threadId] ? true : this.PingHost("127.0.0.1", 1080 + threadId));
                        }
                        if (!flag)
                        {
                            break;
                        }
                        Thread.Sleep(5000);
                    }
                }
                else if ((!this.cbLinkSSH.Checked ? false : !this._isUpdateSSHRunning))
                {
                    lock (Form1.lockObj2)
                    {
                        this._isUpdateSSHRunning = true;
                        this.updateRow(threadId, this.styleBAD, "Updating ssh", "Stop");
                        bool flag2 = this.downloadSSH();
                        this._isUpdateSSHRunning = false;
                        if (!flag2)
                        {
                            this.updateRow(threadId, this.styleBAD, "ERROR SSH DOWNLOAD", "Change");
                            break;
                        }
                    }
                }
                else if ((!this.cbLinkSSH.Checked ? true : !this._isUpdateSSHRunning))
                {
                    this.updateRow(threadId, this.styleBAD, "Stop, SSH out of stock.", "Change");
                    break;
                }
                else
                {
                    while (this._isUpdateSSHRunning)
                    {
                        this.updateRow(threadId, this.styleBAD, "Wait for update ssh", "Stop");
                        Thread.Sleep(1000);
                    }
                }
            }
            if (this._stopFlag)
            {
                this.updateRow(threadId, this.styleBAD, "Stopped", "Change");
            }
            this._isChangingSSH[threadId] = false;
        }
        private void unRar(string fromFile, string toFolder, bool overwrite = true)
        {
            if (!Directory.Exists(toFolder))
            {
                Directory.CreateDirectory(toFolder);
            }
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = this.winrarDir;
            if (!overwrite)
            {
                process.StartInfo.Arguments = string.Format("x -o- -ibck \"{0}\" \"{1}\"", fromFile, toFolder);
            }
            else
            {
                process.StartInfo.Arguments = string.Format("x -o+ -ibck \"{0}\" \"{1}\"", fromFile, toFolder);
            }
            process.Start();
            process.WaitForExit();
        }
        private void updateRow(int rowID, DataGridViewCellStyle style, string status, string buttonValue)
        {
            lock (Form1.lockObj3rowupdate)
            {
                this.dataGridView1.Rows[rowID].Cells[0].Style = style;
                this.dataGridView1.Rows[rowID].Cells[2].Value = status;
                this.dataGridView1.Rows[rowID].Cells[3].Value = buttonValue;
            }
        }
    }
}
