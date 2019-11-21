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
    public partial class Form2 : Form
    {
        private Label label1;
        private Label label2;
        private TextBox txtKey;
        private Button btnActive;

        public string key
        {
            get
            {
                return this.txtKey.Text;
            }
            set
            {
                this.txtKey.Text = value;
            }
        }

        public Form2()
        {
            this.label1 = new Label();
            this.label2 = new Label();
            this.txtKey = new TextBox();
            this.btnActive = new Button();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new Size(202, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vui lòng nhập key để kích hoạt";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(13, 53);
            this.label2.Name = "label2";
            this.label2.Size = new Size(36, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Key:";
            this.txtKey.Location = new Point(56, 53);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new Size(317, 22);
            this.txtKey.TabIndex = 2;
            this.btnActive.Location = new Point(154, 96);
            this.btnActive.Name = "btnActive";
            this.btnActive.Size = new Size(75, 40);
            this.btnActive.TabIndex = 3;
            this.btnActive.Text = "Active";
            this.btnActive.UseVisualStyleBackColor = true;
            this.btnActive.Click += new EventHandler(this.btnActive_Click);
            base.AutoScaleDimensions = new SizeF(8f, 16f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(391, 148);
            base.Controls.Add(this.btnActive);
            base.Controls.Add(this.txtKey);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Name = "Form2";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Active";
            base.Load += new EventHandler(this.Form2_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }
        private void btnActive_Click(object sender, EventArgs e)
        {
            base.Close();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.txtKey.PasswordChar = '*';
        }
    }
}
