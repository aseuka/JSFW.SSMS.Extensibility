using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        private string _dbConnectionString = string.Empty;

        public ColumnInfomationViewForm()
        {
            InitializeComponent(); 
        }

        internal void DataBind(string objectID, string objectName, DataSet ds, string dbConnectionString)
        {
            ObjectID = objectID;
            ObjectName = objectName;
            txtTableName.Text = objectName;
            txtTableDesc.Text = "";
            _dbConnectionString = dbConnectionString;

            if (!string.IsNullOrWhiteSpace(objectName) && !string.IsNullOrWhiteSpace(dbConnectionString))
            {
                try
                {
                    using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(dbConnectionString)) 
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = $@"
SELECT TOP 1 E.VALUE AS [DESC] 
  FROM SYS.EXTENDED_PROPERTIES E 
 WHERE MAJOR_ID = {objectID}
   AND MINOR_ID = 0 
   AND CLASS = 1 AND class_desc = 'OBJECT_OR_COLUMN'
   AND [NAME] = 'MS_Description'";
                            cmd.CommandTimeout = 2;
                            con.Open();
                            object desc = cmd.ExecuteScalar();
                            txtTableDesc.Text = $"{desc}";
                        }
                    }
                }
                catch (Exception queryEx)
                {

                }
            }
            txtTableDesc.Modified = false;

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
            text_colID.ReadOnly = true;
            dataGridView1.Columns.Add(text_colID);

            DataGridViewTextBoxColumn text_name = new DataGridViewTextBoxColumn();
            text_name.HeaderText = "Column Name";
            text_name.Name = text_name.DataPropertyName = "NAME";
            text_name.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_name.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_name.Width = 240;
            text_name.ReadOnly = true;
            text_name.Visible = true;
            dataGridView1.Columns.Add(text_name);

            DataGridViewTextBoxColumn text_db1_type_name = new DataGridViewTextBoxColumn();
            text_db1_type_name.HeaderText = "타입";
            text_db1_type_name.Name = text_db1_type_name.DataPropertyName = "TYPE_NAME";
            text_db1_type_name.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_db1_type_name.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_type_name.Width = 100;
            text_db1_type_name.Visible = true;
            text_db1_type_name.ReadOnly = true;
            dataGridView1.Columns.Add(text_db1_type_name);

            DataGridViewTextBoxColumn text_db1_desc = new DataGridViewTextBoxColumn();
            text_db1_desc.HeaderText = "설명";
            text_db1_desc.Name = text_db1_desc.DataPropertyName = "DESC";
            text_db1_desc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            text_db1_desc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;            
            text_db1_desc.Width = 160;
            text_db1_desc.ReadOnly = false;
            text_db1_desc.Visible = true;            
            dataGridView1.Columns.Add(text_db1_desc);

            DataGridViewTextBoxColumn text_db1_key = new DataGridViewTextBoxColumn();
            text_db1_key.HeaderText = "null/key";
            text_db1_key.Name = text_db1_key.DataPropertyName = "NULL_KEY_TYPE";
            text_db1_key.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_key.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_key.Width = 90;
            text_db1_key.ReadOnly = true;
            text_db1_key.Visible = true;
            dataGridView1.Columns.Add(text_db1_key);

            DataGridViewTextBoxColumn text_db1_collation = new DataGridViewTextBoxColumn();
            text_db1_collation.HeaderText = "정렬";
            text_db1_collation.Name = text_db1_collation.DataPropertyName = "COLLATION";
            text_db1_collation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_collation.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            text_db1_collation.Width = 60;
            text_db1_collation.ReadOnly = true;
            text_db1_collation.Visible = true;
            dataGridView1.Columns.Add(text_db1_collation);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].TableName;
            dataGridView1.ReadOnly = true;
        }

        private void txtTableDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                txtTableDesc.Modified = false;
                txtTableDesc.ReadOnly = false;
                txtTableDesc.Focus();
            }
            else if (e.KeyCode == Keys.Escape || e.KeyData == Keys.Enter)
            {
                txtTableDesc.ReadOnly = true;
                if (txtTableDesc.Modified)
                {
                    try
                    {
                        string query = UpdateTableExtendScript(txtTableName.Text?.Trim(), txtTableDesc.Text?.Trim());
                        if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(_dbConnectionString))
                        {
                            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_dbConnectionString))
                            {
                                using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = query;
                                    cmd.CommandTimeout = 3;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (Exception eSqlEx)
                    {
                    }
                    finally
                    {
                        txtTableDesc.Modified = false;
                    }
                }
            }
        }
        
        private void txtTableDesc_DoubleClick(object sender, EventArgs e)
        {
            SendKeys.Send("{F2}");
            SendKeys.Flush();
        }

        bool isCellValueChanged = false;
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            isCellValueChanged = true;
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                dataGridView1.EndEdit();
                dataGridView1.ReadOnly = true;
                if (0 <= e.RowIndex && 0 <= e.ColumnIndex && !string.IsNullOrWhiteSpace(_dbConnectionString))
                {
                    DataGridViewTextBoxCell text_db1_desc = dataGridView1[e.ColumnIndex, e.RowIndex] as DataGridViewTextBoxCell;
                    if (text_db1_desc != null && text_db1_desc.OwningColumn.Name == "DESC" && isCellValueChanged)
                    {
                        DataGridViewTextBoxCell text_name = dataGridView1["NAME", e.RowIndex] as DataGridViewTextBoxCell;

                        string query = UpdateColumnExtendScript(txtTableName.Text?.Trim(), $"{text_name.Value}".Trim(), $"{text_db1_desc.Value}");
                        if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(_dbConnectionString))
                        {
                            try
                            {
                                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_dbConnectionString))
                                {
                                    using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandType = CommandType.Text;
                                        cmd.CommandText = query;
                                        cmd.CommandTimeout = 3;
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            catch (Exception eSqlex)
                            {
                            }
                        }
                    }
                }
            }
            finally
            {
                isCellValueChanged = false;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (0 <= e.RowIndex && 0 <= e.ColumnIndex)
            {
                DataGridViewTextBoxCell text_db1_desc = dataGridView1[e.ColumnIndex, e.RowIndex] as DataGridViewTextBoxCell;
                if (text_db1_desc != null && text_db1_desc.OwningColumn.Name == "DESC")
                {
                    dataGridView1.ReadOnly = false;
                    text_db1_desc.ReadOnly = false;
                    dataGridView1.BeginEdit(false);
                }
            }
        }
        
        private string UpdateTableExtendScript(string tableName, string dbdesc, string schema = "dbo")
        {
            if (string.IsNullOrWhiteSpace(tableName)) return "";

            string cmdQuery = $@"
IF EXISTS (
	SELECT TOP 1 E.VALUE AS [DESC] 
  FROM SYS.EXTENDED_PROPERTIES E 
 WHERE MAJOR_ID = OBJECT_ID('{tableName}')
   AND MINOR_ID = 0 
   AND CLASS = 1 AND class_desc = 'OBJECT_OR_COLUMN'
   AND [NAME] = 'MS_Description' )
BEGIN
	
	EXEC sys.sp_dropextendedproperty 
				@name=N'MS_Description' 
			  , @level0type=N'SCHEMA'
			  , @level0name=N'{schema}'
			  , @level1type=N'TABLE'
			  , @level1name=N'{tableName}'
END 

EXEC sys.sp_addextendedproperty 
				@name=N'MS_Description'
			  , @value=N'{dbdesc}' 
			  , @level0type=N'SCHEMA'
			  , @level0name=N'{schema}'
			  , @level1type=N'TABLE'
			  , @level1name=N'{tableName}'
";
            return cmdQuery;        
        }


        private string UpdateColumnExtendScript(string tableName, string colName, string dbdesc, string schema = "dbo")
        {
            string cmdQuery = $@"
IF EXISTS (
	SELECT TOP 1 E.VALUE AS [DESC] 
  FROM SYS.EXTENDED_PROPERTIES E INNER JOIN SYS.COLUMNS C
    ON E.MAJOR_ID = C.OBJECT_ID AND E.MINOR_ID = C.COLUMN_ID
 WHERE MAJOR_ID = OBJECT_ID('{tableName}')
   AND C.NAME = '{colName}'
   AND class_desc = 'OBJECT_OR_COLUMN'
   AND E.[NAME] = 'MS_Description' )
BEGIN
	
	EXEC sys.sp_dropextendedproperty 
				@name=N'MS_Description' 
			  , @level0type=N'SCHEMA'
			  , @level0name=N'{schema}'
			  , @level1type=N'TABLE'
			  , @level1name=N'{tableName}'
			  , @level2type=N'COLUMN'
			  , @level2name=N'{colName}'
END 


EXEC sys.sp_addextendedproperty 
				@name=N'MS_Description'
			  , @value=N'{dbdesc}'
			  , @level0type=N'SCHEMA'
			  , @level0name=N'{schema}'
			  , @level1type=N'TABLE'
			  , @level1name=N'{tableName}'
			  , @level2type=N'COLUMN'
			  , @level2name=N'{colName}'
"; 
            return cmdQuery;
        }

    }
}
