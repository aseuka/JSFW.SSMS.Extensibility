namespace JSFW
{
    partial class CreateCodeTmpForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.cboGroup = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbTime = new System.Windows.Forms.Label();
            this.lbDay = new System.Windows.Forms.Label();
            this.lbCode = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.lbOldFileName = new System.Windows.Forms.Label();
            this.btnAddTemp = new System.Windows.Forms.Button();
            this.txtTempliteName = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.btnGet = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.txtGroupName);
            this.panel1.Controls.Add(this.btnAddGroup);
            this.panel1.Controls.Add(this.cboGroup);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 636);
            this.panel1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(5, 59);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(284, 568);
            this.listBox1.TabIndex = 4;
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // txtGroupName
            // 
            this.txtGroupName.Location = new System.Drawing.Point(5, 12);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(190, 21);
            this.txtGroupName.TabIndex = 3;
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Location = new System.Drawing.Point(197, 11);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(92, 23);
            this.btnAddGroup.TabIndex = 2;
            this.btnAddGroup.Text = "그룹 저장";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // cboGroup
            // 
            this.cboGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGroup.FormattingEnabled = true;
            this.cboGroup.Location = new System.Drawing.Point(5, 36);
            this.cboGroup.Name = "cboGroup";
            this.cboGroup.Size = new System.Drawing.Size(284, 20);
            this.cboGroup.TabIndex = 0;
            this.cboGroup.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnGet);
            this.panel2.Controls.Add(this.lbTime);
            this.panel2.Controls.Add(this.lbDay);
            this.panel2.Controls.Add(this.lbCode);
            this.panel2.Controls.Add(this.btnNew);
            this.panel2.Controls.Add(this.lbOldFileName);
            this.panel2.Controls.Add(this.btnAddTemp);
            this.panel2.Controls.Add(this.txtTempliteName);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(295, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(896, 636);
            this.panel2.TabIndex = 1;
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Font = new System.Drawing.Font("굴림체", 9F);
            this.lbTime.Location = new System.Drawing.Point(714, 32);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(83, 24);
            this.lbTime.TabIndex = 5;
            this.lbTime.Text = "시간\r\n$CHANGE_TIME$";
            this.lbTime.DoubleClick += new System.EventHandler(this.lbTime_DoubleClick);
            // 
            // lbDay
            // 
            this.lbDay.AutoSize = true;
            this.lbDay.Font = new System.Drawing.Font("굴림체", 9F);
            this.lbDay.Location = new System.Drawing.Point(631, 32);
            this.lbDay.Name = "lbDay";
            this.lbDay.Size = new System.Drawing.Size(77, 24);
            this.lbDay.TabIndex = 5;
            this.lbDay.Text = "일자\r\n$CHANGE_DAY$";
            this.lbDay.DoubleClick += new System.EventHandler(this.lbDay_DoubleClick);
            // 
            // lbCode
            // 
            this.lbCode.AutoSize = true;
            this.lbCode.Font = new System.Drawing.Font("굴림체", 9F);
            this.lbCode.ForeColor = System.Drawing.Color.Silver;
            this.lbCode.Location = new System.Drawing.Point(808, 6);
            this.lbCode.Name = "lbCode";
            this.lbCode.Size = new System.Drawing.Size(83, 24);
            this.lbCode.TabIndex = 5;
            this.lbCode.Text = "변경대상코드\r\n$CHANGE_CODE$";
            this.lbCode.Visible = false;
            this.lbCode.DoubleClick += new System.EventHandler(this.lbCode_DoubleClick);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(461, 35);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 4;
            this.btnNew.Text = "새로 작성";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lbOldFileName
            // 
            this.lbOldFileName.AutoSize = true;
            this.lbOldFileName.Location = new System.Drawing.Point(3, 19);
            this.lbOldFileName.Name = "lbOldFileName";
            this.lbOldFileName.Size = new System.Drawing.Size(84, 12);
            this.lbOldFileName.TabIndex = 3;
            this.lbOldFileName.Text = "old File Name";
            // 
            // btnAddTemp
            // 
            this.btnAddTemp.Location = new System.Drawing.Point(354, 35);
            this.btnAddTemp.Name = "btnAddTemp";
            this.btnAddTemp.Size = new System.Drawing.Size(101, 23);
            this.btnAddTemp.TabIndex = 2;
            this.btnAddTemp.Text = "템플릿 저장";
            this.btnAddTemp.UseVisualStyleBackColor = true;
            this.btnAddTemp.Click += new System.EventHandler(this.btnAddTemp_Click);
            // 
            // txtTempliteName
            // 
            this.txtTempliteName.Location = new System.Drawing.Point(2, 36);
            this.txtTempliteName.Name = "txtTempliteName";
            this.txtTempliteName.Size = new System.Drawing.Size(351, 21);
            this.txtTempliteName.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.elementHost1);
            this.panel3.Location = new System.Drawing.Point(2, 59);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(892, 575);
            this.panel3.TabIndex = 0;
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(892, 575);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(541, 35);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(83, 23);
            this.btnGet.TabIndex = 6;
            this.btnGet.Text = "가져오기";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // CreateCodeTmpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1191, 636);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CreateCodeTmpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateCodeTmpForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.ComboBox cboGroup;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnAddTemp;
        private System.Windows.Forms.TextBox txtTempliteName;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Label lbOldFileName;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lbCode;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label lbDay;
        private System.Windows.Forms.Button btnGet;
    }
}