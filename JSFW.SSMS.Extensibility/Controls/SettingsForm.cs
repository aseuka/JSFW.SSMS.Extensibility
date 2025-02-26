using JSFW.SSMS.Extensibility.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JSFW
{
    public partial class SettingsForm : Form
    {
        bool IsModify = false;
        public SettingsForm()
        {
            InitializeComponent(); 
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadConfig();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (IsModify && MessageBox.Show("변경내용있음. 저장?", "저장?",  MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveConfig();
            }
            base.OnFormClosing(e);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            // todo : 설정 취소
            IsModify = false;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // todo : 설정 저장.
            /*
            JSFW.NEWFMT.Properties.Settings.Default.KeywordUpper = KeywordUpperRadio.Checked;
            JSFW.NEWFMT.Properties.Settings.Default.KeywordAlign = LeftAlignRadio.Checked;
            JSFW.NEWFMT.Properties.Settings.Default.CommaPosition = CommaBeforeRadio.Checked; 
             */
            SaveConfig();
            IsModify = false;
            this.Close();
        }

        private void SaveConfig()
        {
            Settings.Default.CleanComment = chkCleanComment.Checked;
            Settings.Default.KeywordUpper = KeywordUpperRadio.Checked;
            Settings.Default.KeywordAlign = LeftAlignRadio.Checked;
            Settings.Default.CommaPosition = CommaBeforeRadio.Checked;
            Settings.Default.Save();
        }
         
        private void LoadConfig()
        {
            Settings.Default.Reload();
            chkCleanComment.Checked = Settings.Default.CleanComment;
            KeywordUpperRadio.Checked = Settings.Default.KeywordUpper;
            KeywordLowerRadio.Checked = !KeywordUpperRadio.Checked;
            LeftAlignRadio.Checked = Settings.Default.KeywordAlign;
            RightAlignRadio.Checked = !LeftAlignRadio.Checked;
            CommaBeforeRadio.Checked = Settings.Default.CommaPosition;
            CommaAfterRadio.Checked = !CommaBeforeRadio.Checked;
            IsModify = false;
        }

        private void KeywordUpperRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsModify = true;
        }

        private void KeywordLowerRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsModify = true;
        }

        private void LeftAlignRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsModify = true;
        }

        private void RightAlignRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsModify = true;
        }

        private void CommaBeforeRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsModify = true;
        }

        private void CommaAfterRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsModify = true;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
