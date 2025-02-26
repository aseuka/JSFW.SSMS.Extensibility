namespace JSFW
{
    partial class ObjectFindForm
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
            this.components = new System.ComponentModel.Container();
            this.btnFind = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnHelpTxt = new System.Windows.Forms.Button();
            this.btnTableSelect = new System.Windows.Forms.Button();
            this.btnColumnInfo = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.조회프로시져ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cUD프로시져ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.복사ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.컬럼정보보기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cboFindText = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.Location = new System.Drawing.Point(172, 6);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(47, 23);
            this.btnFind.TabIndex = 0;
            this.btnFind.Text = "찾기";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "검색";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(210, 181);
            this.dataGridView1.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(8, 31);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnHelpTxt);
            this.splitContainer1.Panel2.Controls.Add(this.btnTableSelect);
            this.splitContainer1.Panel2.Controls.Add(this.btnColumnInfo);
            this.splitContainer1.Size = new System.Drawing.Size(210, 427);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 4;
            // 
            // btnHelpTxt
            // 
            this.btnHelpTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelpTxt.Location = new System.Drawing.Point(5, 61);
            this.btnHelpTxt.Name = "btnHelpTxt";
            this.btnHelpTxt.Size = new System.Drawing.Size(202, 23);
            this.btnHelpTxt.TabIndex = 0;
            this.btnHelpTxt.Text = "SP_HELPTXT";
            this.btnHelpTxt.UseVisualStyleBackColor = true;
            this.btnHelpTxt.Click += new System.EventHandler(this.btnHelpTxt_Click);
            // 
            // btnTableSelect
            // 
            this.btnTableSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTableSelect.Location = new System.Drawing.Point(5, 32);
            this.btnTableSelect.Name = "btnTableSelect";
            this.btnTableSelect.Size = new System.Drawing.Size(202, 23);
            this.btnTableSelect.TabIndex = 0;
            this.btnTableSelect.Text = "TABLE_SELECT";
            this.btnTableSelect.UseVisualStyleBackColor = true;
            this.btnTableSelect.Click += new System.EventHandler(this.btnTableSelect_Click);
            // 
            // btnColumnInfo
            // 
            this.btnColumnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColumnInfo.Location = new System.Drawing.Point(5, 3);
            this.btnColumnInfo.Name = "btnColumnInfo";
            this.btnColumnInfo.Size = new System.Drawing.Size(202, 23);
            this.btnColumnInfo.TabIndex = 0;
            this.btnColumnInfo.Text = "COLUMN_INFO";
            this.btnColumnInfo.UseVisualStyleBackColor = true;
            this.btnColumnInfo.Click += new System.EventHandler(this.btnColumnInfo_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.조회프로시져ToolStripMenuItem,
            this.cUD프로시져ToolStripMenuItem,
            this.복사ToolStripMenuItem,
            this.컬럼정보보기ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(152, 92);
            // 
            // 조회프로시져ToolStripMenuItem
            // 
            this.조회프로시져ToolStripMenuItem.Name = "조회프로시져ToolStripMenuItem";
            this.조회프로시져ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.조회프로시져ToolStripMenuItem.Text = "R 프로시져";
            this.조회프로시져ToolStripMenuItem.Visible = false;
            this.조회프로시져ToolStripMenuItem.Click += new System.EventHandler(this.조회프로시져ToolStripMenuItem_Click);
            // 
            // cUD프로시져ToolStripMenuItem
            // 
            this.cUD프로시져ToolStripMenuItem.Name = "cUD프로시져ToolStripMenuItem";
            this.cUD프로시져ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.cUD프로시져ToolStripMenuItem.Text = "CUD 프로시져";
            this.cUD프로시져ToolStripMenuItem.Visible = false;
            this.cUD프로시져ToolStripMenuItem.Click += new System.EventHandler(this.cUD프로시져ToolStripMenuItem_Click);
            // 
            // 복사ToolStripMenuItem
            // 
            this.복사ToolStripMenuItem.Name = "복사ToolStripMenuItem";
            this.복사ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.복사ToolStripMenuItem.Text = "복사";
            this.복사ToolStripMenuItem.Visible = false;
            this.복사ToolStripMenuItem.Click += new System.EventHandler(this.복사ToolStripMenuItem_Click);
            // 
            // 컬럼정보보기ToolStripMenuItem
            // 
            this.컬럼정보보기ToolStripMenuItem.Name = "컬럼정보보기ToolStripMenuItem";
            this.컬럼정보보기ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.컬럼정보보기ToolStripMenuItem.Text = "컬럼정보 보기";
            this.컬럼정보보기ToolStripMenuItem.Visible = false;
            this.컬럼정보보기ToolStripMenuItem.Click += new System.EventHandler(this.컬럼정보보기ToolStripMenuItem_Click);
            // 
            // cboFindText
            // 
            this.cboFindText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFindText.FormattingEnabled = true;
            this.cboFindText.Location = new System.Drawing.Point(47, 7);
            this.cboFindText.Name = "cboFindText";
            this.cboFindText.Size = new System.Drawing.Size(121, 20);
            this.cboFindText.TabIndex = 5;
            // 
            // ObjectFindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 465);
            this.Controls.Add(this.cboFindText);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "ObjectFindForm";
            this.Text = "오브젝트 파인더";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnHelpTxt;
        private System.Windows.Forms.Button btnTableSelect;
        private System.Windows.Forms.Button btnColumnInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 조회프로시져ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cUD프로시져ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 복사ToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboFindText;
        private System.Windows.Forms.ToolStripMenuItem 컬럼정보보기ToolStripMenuItem;
    }
}