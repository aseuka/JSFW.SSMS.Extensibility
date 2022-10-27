using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSFW
{
    public partial class CreateCodeTmpForm : Form
    {
        /// <summary>
        /// { D:\\, C:\\ }JSFW\\SQL_CodeTemplate\\"
        /// </summary>
        internal static string Root = JSFW.SSMS.Extensibility.VSPackage._DIR_JSFW + "SQL_CodeTemplate\\";

        internal Func<string> ImportToString = null;

        ICSharpCode.AvalonEdit.TextEditor txtContent = new ICSharpCode.AvalonEdit.TextEditor()
        {
            SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("MarkDown"),
            FontSize = 14,
            AllowDrop = true,
        };

        public CreateCodeTmpForm()
        {
            InitializeComponent();

            txtTempliteName.ReadOnly = true;

            try
            {
                txtContent.FontFamily = new System.Windows.Media.FontFamily("Ubuntu Mono");
            }
            catch
            {
                txtContent.FontFamily = new System.Windows.Media.FontFamily("굴림체");
            }

            txtContent.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(txtContent.Options);
            txtContent.ShowLineNumbers = true;

            elementHost1.Child = txtContent;

            this.Disposed += CreateCodeTmpForm_Disposed;
            
            LoadTemp();
        }

        private void LoadTemp()
        {
            DataClear();

            cboGroup.Items.Clear();

            foreach (var dir in System.IO.Directory.GetDirectories(Root))
            {
                cboGroup.Items.Add(dir.Replace(Root, "").Trim('\\'));
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear
            LoadTemplite();
        }

        private void LoadTemplite()
        {
            DataClear();

            string groupName = cboGroup.Text;
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                string dir = Root + groupName;
                foreach (var file in Directory.GetFiles(dir))
                {
                    listBox1.Items.Add(Path.GetFileName(file));
                }
            }
        }

        private void DataClear()
        {
            listBox1.Items.Clear();

            lbOldFileName.Text = "";
            txtTempliteName.Text = "";
            txtContent.Text = "";
        }

        private void CreateCodeTmpForm_Disposed(object sender, EventArgs e)
        {
            ImportToString = null;
            txtContent = null;
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            // 그룹 저장
            if (string.IsNullOrWhiteSpace(txtGroupName.Text)) return;

            string dirName = txtGroupName.Text.Trim();
            foreach (var ch in Path.GetInvalidPathChars())
            {
                dirName = dirName.Replace(""+ch, "");
            }

            if (Directory.Exists(Root + dirName) == false) Directory.CreateDirectory(Root + dirName);

            LoadTemp();
            cboGroup.SelectedItem = dirName;

            txtGroupName.Clear();
        }
         
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string fileName = "" + listBox1.SelectedItem;
            if (string.IsNullOrWhiteSpace(fileName)) return;
            txtTempliteName.Text = fileName.Substring(0, fileName.Length - ".txt".Length);

            lbOldFileName.Text = Root + cboGroup.Text + "\\" + fileName;
            string txt = File.ReadAllText( Root  + cboGroup.Text + "\\" + fileName, Encoding.UTF8);
            txtContent.Text = txt;
        }

        private void btnAddTemp_Click(object sender, EventArgs e)
        {
            // 템플릿 저장
            try
            {
                bool isNew = string.IsNullOrWhiteSpace(lbOldFileName.Text);

                string fileName = txtTempliteName.Text;
                foreach (var ch in Path.GetInvalidFileNameChars())
                {
                    fileName = fileName.Replace("" + ch, "");
                } 

                if (string.IsNullOrWhiteSpace( fileName))
                {
                    MessageBox.Show("쿼리 저장 파일명이 필수.");
                    txtTempliteName.Focus();
                    return;
                }

                fileName = Root + cboGroup.Text + "\\" + fileName;

                if (isNew)
                {
                    fileName += ".txt";
                }
                else
                {
                    fileName = lbOldFileName.Text;
                }
                File.WriteAllText(fileName, txtContent.Text, Encoding.UTF8);


                if (isNew)
                {
                    LoadTemplite();

                    listBox1.SelectedItem = Path.GetFileName(fileName);

                    listBox1_MouseDoubleClick(listBox1, null);
                }
            }
            finally
            {
                txtTempliteName.ReadOnly = true; 
            }
        }
         
        private void btnNew_Click(object sender, EventArgs e)
        {
            // 새 템플릿 작성
            lbOldFileName.Text = "";
            txtTempliteName.Text = ""; txtTempliteName.ReadOnly = false;
            txtContent.Text = "";
        }

        private void lbCode_DoubleClick(object sender, EventArgs e)
        {
            txtContent.SelectedText = "$CHANGE_CODE$";
        }

        private void lbDay_DoubleClick(object sender, EventArgs e)
        {
            txtContent.SelectedText = "$CHANGE_DAY$";
        }

        private void lbTime_DoubleClick(object sender, EventArgs e)
        {
            txtContent.SelectedText = "$CHANGE_TIME$";
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            txtContent.SelectedText = ImportToString?.Invoke() ?? "";
        }
    }
}
