using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.UI.Grid;

namespace JSFW
{
    /// <summary>
    /// 컬럼 검색 창
    /// </summary>
    public partial class FindColumnNameForm : Form
    {
        IGridControl Grid { get; set; }
        private FindColumnNameForm()
        {
            InitializeComponent();
        }

        public FindColumnNameForm(IGridControl grid) : this()
        {
            OldInfo = null;
            Grid = grid;
            this.Disposed += FindColumnName_Disposed; 
        }

        void FindColumnName_Disposed(object sender, EventArgs e)
        {
            Grid = null;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            textBox1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 찾기!
            if (string.IsNullOrEmpty(textBox1.Text.Trim())) return;

            Microsoft.SqlServer.Management.UI.Grid.GridControl GRD = Grid as Microsoft.SqlServer.Management.UI.Grid.GridControl;
            GridHeader headers = GetNonPublicField(GRD, "m_gridHeader") as GridHeader;

            try
            { 
                // Header 가져오는 소스
                string hd = "";
                System.Drawing.Bitmap bmp;
                //grid.GetHeaderInfo(1, out hd, out bmp);  

                if (OldInfo != null)
                {
                    if (headers != null) headers.SetHeaderItemState(OldInfo.Index, false);
                }
                OldInfo = null; // 초기화가... 안되어 덮어쓰기가 되네.
                listBox1.BeginUpdate();
                listBox1.SelectedIndex = -1;
                listBox1.Items.Clear();

                string findWord = textBox1.Text.Trim();

                for (int col = 1; col < Grid.ColumnsNumber; col++)
                {
                    Grid.GetHeaderInfo(col, out hd, out bmp);

                    string originalHD = hd;
                    if (string.IsNullOrEmpty(hd.Trim())) continue;

                    if (hd.Trim() == "(열 이름 없음)")
                    {
                        hd = "unknown";
                    }

                    if (hd.TrimEnd().ToUpper().Contains(findWord.ToUpper()))
                    {
                        listBox1.Items.Add(new FindItemInfo() { Index = col, ColumnName = hd.Trim(), OriginalHD = originalHD, OriginalWidthType = Grid.GetGridColumnInfo(col).WidthType, OriginalWidth = Grid.GetColumnWidth(col) });
                    }
                    if (headers != null) headers.SetHeaderItemState(col, false); 
                }
                listBox1.EndUpdate();
                listBox1.ValueMember = "Index";
                listBox1.DisplayMember = "ColumnName";
            }
            finally
            {
                GRD.UpdateGrid(true);
                headers = null;
            }
        }

        /// <summary>
        /// 컬럼 검색정보
        /// </summary>
        public class FindItemInfo
        {
            /// <summary>
            /// 인덱스
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// 처음 헤더값
            /// </summary>
            public string OriginalHD { get; set; }
            /// <summary>
            /// 컬럼명
            /// </summary>
            public string ColumnName { get; set; }

            /// <summary>
            /// 처음 컬럼 폭
            /// </summary>
            public int OriginalWidth { get; set; }

            /// <summary>
            /// 처음 컬럼 폭 타입
            /// </summary>
            public GridColumnWidthType OriginalWidthType { get; set; }
        }
         
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Microsoft.SqlServer.Management.UI.Grid.GridControl GRD = Grid as Microsoft.SqlServer.Management.UI.Grid.GridControl;
            GridHeader headers = GetNonPublicField(GRD, "m_gridHeader") as GridHeader;
            try
            {
                for (int loop = listBox1.Items.Count - 1; loop >= 0; loop--)
                {
                    FindItemInfo info = listBox1.SelectedItem as FindItemInfo;
                    if (info == null) continue;

                    if (headers != null) headers.SetHeaderItemState(info.Index, false);
                }
                listBox1.Items.Clear();
            }
            finally
            {
                GRD.UpdateGrid(true);
                headers = null;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                button1_Click(button1, e);
            }
        }

        FindItemInfo OldInfo = null;

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            FindItemInfo info = listBox1.SelectedItem as FindItemInfo;
            if (info == null) return;

            Microsoft.SqlServer.Management.UI.Grid.GridControl GRD = Grid as Microsoft.SqlServer.Management.UI.Grid.GridControl;
            GridHeader headers = GetNonPublicField(GRD, "m_gridHeader") as GridHeader; 
            if (OldInfo != null)
            {
                if (headers != null) headers.SetHeaderItemState(OldInfo.Index, false);
            }

            Grid.SelectedCells.Clear(); 
            try
            {
                if (headers != null) headers.SetHeaderItemState(info.Index, true);
                if (0 < Grid.VisibleRowsNum)
                {
                    Grid.SelectedCells.Add(new BlockOfCells(GRD.FirstScrollableRow, info.Index));
                    Grid.EnsureCellIsVisible(GRD.FirstScrollableRow, info.Index); 
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {  
                GRD.UpdateGrid(true);
                headers = null;
            }
            OldInfo = info; 
        }

        private object GetNonPublicField(object obj, string propertyName)
        {
            System.Reflection.FieldInfo p = obj.GetType().GetField(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
            return p.GetValue(obj);
        }
    }
}
