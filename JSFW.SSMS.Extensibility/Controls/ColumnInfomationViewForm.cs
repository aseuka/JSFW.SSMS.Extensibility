using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSFW.SSMS.Extensibility.Controls
{
    public partial class ColumnInfomationViewForm : Form
    {
        public string ObjectID { get; private set; }
        public string ObjectName { get; private set; }

        public ColumnInfomationViewForm()
        {
            InitializeComponent(); 
        }

        internal void DataBind(string objectID, string objectName, DataSet ds)
        {
            ObjectID = objectID;
            ObjectName = objectName;
            textBox1.Text = objectName;

            using (dataGridView1.DataSource as IDisposable)
            {
                dataGridView1.Columns.Clear();
            }
            dataGridView1.DataSource = null;



            DataGridViewTextBoxColumn text_colID = new DataGridViewTextBoxColumn();
            text_colID.HeaderText = "ID";
            text_colID.Name = text_colID.DataPropertyName = "COLID";
            text_colID.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_colID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_colID.Width = 40;
            text_colID.Visible = true;
            dataGridView1.Columns.Add(text_colID);

            DataGridViewTextBoxColumn text_name = new DataGridViewTextBoxColumn();
            text_name.HeaderText = "Column Name";
            text_name.Name = text_name.DataPropertyName = "NAME";
            text_name.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_name.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_name.Width = 240;
            text_name.Visible = true;
            dataGridView1.Columns.Add(text_name);

            DataGridViewTextBoxColumn text_db1_type_name = new DataGridViewTextBoxColumn();
            text_db1_type_name.HeaderText = "타입";
            text_db1_type_name.Name = text_db1_type_name.DataPropertyName = "TYPE_NAME";
            text_db1_type_name.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_db1_type_name.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_type_name.Width = 100;
            text_db1_type_name.Visible = true;
            dataGridView1.Columns.Add(text_db1_type_name);

            DataGridViewTextBoxColumn text_db1_desc = new DataGridViewTextBoxColumn();
            text_db1_desc.HeaderText = "설명";
            text_db1_desc.Name = text_db1_desc.DataPropertyName = "DESC";
            text_db1_desc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_db1_desc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_desc.Width = 160;
            text_db1_desc.Visible = true;
            dataGridView1.Columns.Add(text_db1_desc);

            DataGridViewTextBoxColumn text_db1_key = new DataGridViewTextBoxColumn();
            text_db1_key.HeaderText = "null/key";
            text_db1_key.Name = text_db1_key.DataPropertyName = "NULL_KEY_TYPE";
            text_db1_key.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_key.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_key.Width = 90;
            text_db1_key.Visible = true;
            dataGridView1.Columns.Add(text_db1_key);

            DataGridViewTextBoxColumn text_db1_collation = new DataGridViewTextBoxColumn();
            text_db1_collation.HeaderText = "정렬";
            text_db1_collation.Name = text_db1_collation.DataPropertyName = "COLLATION";
            text_db1_collation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_collation.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_collation.Width = 60;
            text_db1_collation.Visible = true;
            dataGridView1.Columns.Add(text_db1_collation);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].TableName;
            dataGridView1.ReadOnly = true;
        }
    }
}
