using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace thread
{
    public partial class Setting : Form
    {
        private GroupBox groupBox1;
        private CheckBox cbError;
        private CheckBox cbBlacklist;
        private CheckBox cbAnonymizer;
        private CheckBox cbProxy;
        private GroupBox groupBox2;
        private CheckBox cbRandomSSH;
        private CheckBox cbDeleteSSHDieInOriginalIle;
        private CheckBox cbAutoSaveSSHUnused;
        private CheckBox cbFresh;
        private CheckBox cbDuplicate;
        private GroupBox groupBox3;
        private RadioButton rbtnFFProfileThreadId;
        private RadioButton rbtnFFProfileScan;
        public bool Anonymizer
        {
            get { return this.cbAnonymizer.Checked; }
            set { this.cbAnonymizer.Checked = value; }
        }
        public bool Blacklist
        {
            get
            {
                return this.cbBlacklist.Checked;
            }
            set
            {
                this.cbBlacklist.Checked = value;
            }
        }

        public bool DeleteSSHdie
        {
            get
            {
                return this.cbDeleteSSHDieInOriginalIle.Checked;
            }
            set
            {
                this.cbDeleteSSHDieInOriginalIle.Checked = value;
            }
        }

        public bool Duplicate
        {
            get
            {
                return this.cbDuplicate.Checked;
            }
            set
            {
                this.cbDuplicate.Checked = value;
            }
        }

        public bool FFProfileScan
        {
            get
            {
                return this.rbtnFFProfileScan.Checked;
            }
            set
            {
                this.rbtnFFProfileScan.Checked = value;
            }
        }

        public bool FFProfileThreadId
        {
            get
            {
                return this.rbtnFFProfileThreadId.Checked;
            }
            set
            {
                this.rbtnFFProfileThreadId.Checked = value;
            }
        }

        public bool Fresh
        {
            get
            {
                return this.cbFresh.Checked;
            }
            set
            {
                this.cbFresh.Checked = value;
            }
        }

        public bool Proxy
        {
            get
            {
                return this.cbProxy.Checked;
            }
            set
            {
                this.cbProxy.Checked = value;
            }
        }

        public bool RandomSSH
        {
            get
            {
                return this.cbRandomSSH.Checked;
            }
            set
            {
                this.cbRandomSSH.Checked = value;
            }
        }

        public bool Unused
        {
            get
            {
                return this.cbAutoSaveSSHUnused.Checked;
            }
            set
            {
                this.cbAutoSaveSSHUnused.Checked = value;
            }
        }

        public bool WhoerError
        {
            get
            {
                return this.cbError.Checked;
            }
            set
            {
                this.cbError.Checked = value;
            }
        }

        public Setting()
        {
            init();
        }

        private void init()
        {
            this.groupBox1 = new GroupBox();
            this.cbError = new CheckBox();
            this.cbBlacklist = new CheckBox();
            this.cbAnonymizer = new CheckBox();
            this.cbProxy = new CheckBox();
            this.groupBox2 = new GroupBox();
            this.cbRandomSSH = new CheckBox();
            this.cbDeleteSSHDieInOriginalIle = new CheckBox();
            this.cbAutoSaveSSHUnused = new CheckBox();
            this.cbFresh = new CheckBox();
            this.cbDuplicate = new CheckBox();
            this.groupBox3 = new GroupBox();
            this.rbtnFFProfileThreadId = new RadioButton();
            this.rbtnFFProfileScan = new RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            base.SuspendLayout();
            this.groupBox1.Controls.Add(this.cbError);
            this.groupBox1.Controls.Add(this.cbBlacklist);
            this.groupBox1.Controls.Add(this.cbAnonymizer);
            this.groupBox1.Controls.Add(this.cbProxy);
            this.groupBox1.Location = new Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(146, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Whoer.net";
            this.cbError.AutoSize = true;
            this.cbError.Location = new Point(7, 105);
            this.cbError.Name = "cbError";
            this.cbError.Size = new Size(105, 21);
            this.cbError.TabIndex = 3;
            this.cbError.Text = "Next if Error";
            this.cbError.UseVisualStyleBackColor = true;
            this.cbBlacklist.AutoSize = true;
            this.cbBlacklist.Location = new Point(7, 77);
            this.cbBlacklist.Name = "cbBlacklist";
            this.cbBlacklist.Size = new Size(97, 21);
            this.cbBlacklist.TabIndex = 2;
            this.cbBlacklist.Text = "IP Blacklist";
            this.cbBlacklist.UseVisualStyleBackColor = true;
            this.cbAnonymizer.AutoSize = true;
            this.cbAnonymizer.Location = new Point(6, 49);
            this.cbAnonymizer.Name = "cbAnonymizer";
            this.cbAnonymizer.Size = new Size(120, 21);
            this.cbAnonymizer.TabIndex = 1;
            this.cbAnonymizer.Text = "IP Anonymizer";
            this.cbAnonymizer.UseVisualStyleBackColor = true;
            this.cbProxy.AutoSize = true;
            this.cbProxy.Location = new Point(7, 22);
            this.cbProxy.Name = "cbProxy";
            this.cbProxy.Size = new Size(81, 21);
            this.cbProxy.TabIndex = 0;
            this.cbProxy.Text = "IP Proxy";
            this.cbProxy.UseVisualStyleBackColor = true;
            this.groupBox2.Controls.Add(this.cbRandomSSH);
            this.groupBox2.Controls.Add(this.cbDeleteSSHDieInOriginalIle);
            this.groupBox2.Controls.Add(this.cbAutoSaveSSHUnused);
            this.groupBox2.Controls.Add(this.cbFresh);
            this.groupBox2.Controls.Add(this.cbDuplicate);
            this.groupBox2.Location = new Point(166, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(243, 168);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SSH";
            this.cbRandomSSH.AutoSize = true;
            this.cbRandomSSH.Location = new Point(7, 133);
            this.cbRandomSSH.Name = "cbRandomSSH";
            this.cbRandomSSH.Size = new Size(115, 21);
            this.cbRandomSSH.TabIndex = 4;
            this.cbRandomSSH.Text = "Random SSH";
            this.cbRandomSSH.UseVisualStyleBackColor = true;
            this.cbDeleteSSHDieInOriginalIle.AutoSize = true;
            this.cbDeleteSSHDieInOriginalIle.Location = new Point(7, 105);
            this.cbDeleteSSHDieInOriginalIle.Name = "cbDeleteSSHDieInOriginalIle";
            this.cbDeleteSSHDieInOriginalIle.Size = new Size(216, 21);
            this.cbDeleteSSHDieInOriginalIle.TabIndex = 3;
            this.cbDeleteSSHDieInOriginalIle.Text = "Delete SSH die in Original file";
            this.cbDeleteSSHDieInOriginalIle.UseVisualStyleBackColor = true;
            this.cbAutoSaveSSHUnused.AutoSize = true;
            this.cbAutoSaveSSHUnused.Checked = true;
            this.cbAutoSaveSSHUnused.CheckState = CheckState.Checked;
            this.cbAutoSaveSSHUnused.Location = new Point(7, 77);
            this.cbAutoSaveSSHUnused.Name = "cbAutoSaveSSHUnused";
            this.cbAutoSaveSSHUnused.Size = new Size(176, 21);
            this.cbAutoSaveSSHUnused.TabIndex = 2;
            this.cbAutoSaveSSHUnused.Text = "Auto save SSH unused";
            this.cbAutoSaveSSHUnused.UseVisualStyleBackColor = true;
            this.cbFresh.AutoSize = true;
            this.cbFresh.Checked = true;
            this.cbFresh.CheckState = CheckState.Checked;
            this.cbFresh.Location = new Point(7, 49);
            this.cbFresh.Name = "cbFresh";
            this.cbFresh.Size = new Size(109, 21);
            this.cbFresh.TabIndex = 1;
            this.cbFresh.Text = "Check Fresh";
            this.cbFresh.UseVisualStyleBackColor = true;
            this.cbDuplicate.AutoSize = true;
            this.cbDuplicate.Checked = true;
            this.cbDuplicate.CheckState = CheckState.Checked;
            this.cbDuplicate.Location = new Point(7, 22);
            this.cbDuplicate.Name = "cbDuplicate";
            this.cbDuplicate.Size = new Size(132, 21);
            this.cbDuplicate.TabIndex = 0;
            this.cbDuplicate.Text = "Check Duplicate";
            this.cbDuplicate.UseVisualStyleBackColor = true;
            this.groupBox3.Controls.Add(this.rbtnFFProfileThreadId);
            this.groupBox3.Controls.Add(this.rbtnFFProfileScan);
            this.groupBox3.Location = new Point(13, 188);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(396, 108);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Firefox";
            this.rbtnFFProfileThreadId.AutoSize = true;
            this.rbtnFFProfileThreadId.Checked = true;
            this.rbtnFFProfileThreadId.Enabled = false;
            this.rbtnFFProfileThreadId.Location = new Point(7, 50);
            this.rbtnFFProfileThreadId.Name = "rbtnFFProfileThreadId";
            this.rbtnFFProfileThreadId.Size = new Size(148, 21);
            this.rbtnFFProfileThreadId.TabIndex = 1;
            this.rbtnFFProfileThreadId.TabStop = true;
            this.rbtnFFProfileThreadId.Text = "Profile by thread id";
            this.rbtnFFProfileThreadId.UseVisualStyleBackColor = true;
            this.rbtnFFProfileScan.AutoSize = true;
            this.rbtnFFProfileScan.Enabled = false;
            this.rbtnFFProfileScan.Location = new Point(7, 22);
            this.rbtnFFProfileScan.Name = "rbtnFFProfileScan";
            this.rbtnFFProfileScan.Size = new Size(144, 21);
            this.rbtnFFProfileScan.TabIndex = 0;
            this.rbtnFFProfileScan.Text = "Scan exists Profile";
            this.rbtnFFProfileScan.UseVisualStyleBackColor = true;
            base.AutoScaleDimensions = new SizeF(8f, 16f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(431, 308);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.groupBox1);
            base.Name = "Setting";
            this.Text = "Setting";
            base.Load += new EventHandler(this.Setting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            base.ResumeLayout(false);
        }

        private void Setting_Load(object sender, EventArgs e)
        {
        }
    }
}
