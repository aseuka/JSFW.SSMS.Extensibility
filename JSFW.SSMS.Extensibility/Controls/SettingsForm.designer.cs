namespace JSFW
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.CommaAfterRadio = new System.Windows.Forms.RadioButton();
            this.CommaBeforeRadio = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.RightAlignRadio = new System.Windows.Forms.RadioButton();
            this.LeftAlignRadio = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.KeywordUpperRadio = new System.Windows.Forms.RadioButton();
            this.KeywordLowerRadio = new System.Windows.Forms.RadioButton();
            this.chkCleanComment = new System.Windows.Forms.CheckBox();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(139, 79);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "확인";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(220, 79);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "취소";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.CommaAfterRadio);
            this.groupBox6.Controls.Add(this.CommaBeforeRadio);
            this.groupBox6.Location = new System.Drawing.Point(292, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox6.Size = new System.Drawing.Size(134, 61);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "[ 콤마 위치 ]";
            // 
            // CommaAfterRadio
            // 
            this.CommaAfterRadio.AutoSize = true;
            this.CommaAfterRadio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CommaAfterRadio.Checked = true;
            this.CommaAfterRadio.Location = new System.Drawing.Point(15, 38);
            this.CommaAfterRadio.Name = "CommaAfterRadio";
            this.CommaAfterRadio.Size = new System.Drawing.Size(35, 16);
            this.CommaAfterRadio.TabIndex = 0;
            this.CommaAfterRadio.TabStop = true;
            this.CommaAfterRadio.Text = "뒤";
            this.CommaAfterRadio.UseVisualStyleBackColor = true;
            this.CommaAfterRadio.CheckedChanged += new System.EventHandler(this.CommaAfterRadio_CheckedChanged);
            // 
            // CommaBeforeRadio
            // 
            this.CommaBeforeRadio.AutoSize = true;
            this.CommaBeforeRadio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CommaBeforeRadio.Location = new System.Drawing.Point(15, 16);
            this.CommaBeforeRadio.Name = "CommaBeforeRadio";
            this.CommaBeforeRadio.Size = new System.Drawing.Size(35, 16);
            this.CommaBeforeRadio.TabIndex = 0;
            this.CommaBeforeRadio.Text = "앞";
            this.CommaBeforeRadio.UseVisualStyleBackColor = true;
            this.CommaBeforeRadio.CheckedChanged += new System.EventHandler(this.CommaBeforeRadio_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.RightAlignRadio);
            this.groupBox5.Controls.Add(this.LeftAlignRadio);
            this.groupBox5.Location = new System.Drawing.Point(152, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox5.Size = new System.Drawing.Size(134, 61);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "[ 예약어 정렬 ]";
            // 
            // RightAlignRadio
            // 
            this.RightAlignRadio.AutoSize = true;
            this.RightAlignRadio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RightAlignRadio.Checked = true;
            this.RightAlignRadio.Location = new System.Drawing.Point(15, 38);
            this.RightAlignRadio.Name = "RightAlignRadio";
            this.RightAlignRadio.Size = new System.Drawing.Size(59, 16);
            this.RightAlignRadio.TabIndex = 0;
            this.RightAlignRadio.TabStop = true;
            this.RightAlignRadio.Text = "오른쪽";
            this.RightAlignRadio.UseVisualStyleBackColor = true;
            this.RightAlignRadio.CheckedChanged += new System.EventHandler(this.RightAlignRadio_CheckedChanged);
            // 
            // LeftAlignRadio
            // 
            this.LeftAlignRadio.AutoSize = true;
            this.LeftAlignRadio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LeftAlignRadio.Location = new System.Drawing.Point(15, 16);
            this.LeftAlignRadio.Name = "LeftAlignRadio";
            this.LeftAlignRadio.Size = new System.Drawing.Size(47, 16);
            this.LeftAlignRadio.TabIndex = 0;
            this.LeftAlignRadio.Text = "왼쪽";
            this.LeftAlignRadio.UseVisualStyleBackColor = true;
            this.LeftAlignRadio.CheckedChanged += new System.EventHandler(this.LeftAlignRadio_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.KeywordUpperRadio);
            this.groupBox4.Controls.Add(this.KeywordLowerRadio);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox4.Size = new System.Drawing.Size(134, 61);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "[ 예약어 대/소문자 ]";
            // 
            // KeywordUpperRadio
            // 
            this.KeywordUpperRadio.AutoSize = true;
            this.KeywordUpperRadio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.KeywordUpperRadio.Checked = true;
            this.KeywordUpperRadio.Location = new System.Drawing.Point(15, 16);
            this.KeywordUpperRadio.Name = "KeywordUpperRadio";
            this.KeywordUpperRadio.Size = new System.Drawing.Size(59, 16);
            this.KeywordUpperRadio.TabIndex = 0;
            this.KeywordUpperRadio.TabStop = true;
            this.KeywordUpperRadio.Text = "대문자";
            this.KeywordUpperRadio.UseVisualStyleBackColor = true;
            this.KeywordUpperRadio.CheckedChanged += new System.EventHandler(this.KeywordUpperRadio_CheckedChanged);
            // 
            // KeywordLowerRadio
            // 
            this.KeywordLowerRadio.AutoSize = true;
            this.KeywordLowerRadio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.KeywordLowerRadio.Location = new System.Drawing.Point(15, 38);
            this.KeywordLowerRadio.Name = "KeywordLowerRadio";
            this.KeywordLowerRadio.Size = new System.Drawing.Size(59, 16);
            this.KeywordLowerRadio.TabIndex = 0;
            this.KeywordLowerRadio.Text = "소문자";
            this.KeywordLowerRadio.UseVisualStyleBackColor = true;
            this.KeywordLowerRadio.CheckedChanged += new System.EventHandler(this.KeywordLowerRadio_CheckedChanged);
            // 
            // chkCleanComment
            // 
            this.chkCleanComment.Location = new System.Drawing.Point(12, 76);
            this.chkCleanComment.Name = "chkCleanComment";
            this.chkCleanComment.Size = new System.Drawing.Size(104, 24);
            this.chkCleanComment.TabIndex = 4;
            this.chkCleanComment.Text = "주석제거";
            this.chkCleanComment.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 107);
            this.Controls.Add(this.chkCleanComment);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "설정";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton CommaAfterRadio;
        private System.Windows.Forms.RadioButton CommaBeforeRadio;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton RightAlignRadio;
        private System.Windows.Forms.RadioButton LeftAlignRadio;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton KeywordUpperRadio;
        private System.Windows.Forms.RadioButton KeywordLowerRadio;
        private System.Windows.Forms.CheckBox chkCleanComment;
    }
}