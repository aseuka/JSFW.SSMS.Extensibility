using EnvDTE;
using EnvDTE80;
using JSFW.SSMS.Extensibility;
using JSFW.SSMS.Extensibility.Properties;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace JSFW
{
    /// <summary>
    /// 오브젝트 찾기!
    /// </summary>
    public partial class ObjectFindForm : Form 
    {
        static readonly string ObjectFindGUID = "{5894DCAF-30AB-45A7-BCD5-B45B13072227}";

        DTE2 applicationObject { get; set; }
        VSPackage package { get; set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// 오브젝트 찾기!
        /// </summary>
        public ObjectFindForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            splitContainer1.Panel2Collapsed = true;

            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.CellMouseDown += dataGridView1_CellMouseDown;
            FormClosed += ObjectFindForm_FormClosed;
        }

        void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.RowIndex < 0) return;

                string type = "" + dataGridView1["type_desc", e.RowIndex].Value;
                string tbName = "" + dataGridView1["name", e.RowIndex].Value;
                if (type == "USER_TABLE")
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];
                    Point pt = dataGridView1.GetCellDisplayRectangle(2, e.RowIndex, false).Location;

                    조회프로시져ToolStripMenuItem.Visible = true;
                    cUD프로시져ToolStripMenuItem.Visible = true;
                    복사ToolStripMenuItem.Visible = true;

                    contextMenuStrip1.Show(dataGridView1, pt);
                }
                else if (type == "SQL_STORED_PROCEDURE")
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];
                    Point pt = dataGridView1.GetCellDisplayRectangle(2, e.RowIndex, false).Location;

                    조회프로시져ToolStripMenuItem.Visible = false;
                    cUD프로시져ToolStripMenuItem.Visible = false;
                    복사ToolStripMenuItem.Visible = true;

                    contextMenuStrip1.Show(dataGridView1, pt);
                }
            }
        }

        void ObjectFindForm_FormClosed(object sender, FormClosedEventArgs e)
        { 
            applicationObject = null;
            package = null;
        }
        
        #region 솔루션 탐색기와 연계 - 이동
        void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 솔루션 탐색기와 동기화 - 객체로 이동
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var cellColumn = dataGridView1.Columns[e.ColumnIndex];
            var rowData = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowData != null)
            {
                string schema = "" + rowData.Row["schema"];
                string objectName = "" + rowData.Row["name"];
                string type = "" + rowData.Row["type"];
                if (cellColumn is DataGridViewButtonColumn)
                {
                    // sp를 가져와서!!! 수정편집창에 뿌려주기.

                }
                else
                {
                    // 솔루션에서 찾아서 이동!!
                    SyncObjectExplorer(rowData);
                }
            }
        }

        private void SyncObjectExplorer(DataRowView rowData)
        {
            if (rowData == null) return;

            //int cnt = 0;
            //INodeInformation[] r = null ;
            //objExplorer.GetSelectedNodes(out cnt, out r);

            /*
                Server[@Name='DB-SVR']/Database[@Name='SY']/Table[@Name='공종분류$' and @Schema='dbo']/Column[@Name='공사구분']
                Server[@Name='DB-SVR']/Database[@Name='SY']/View[@Name='DAAV_DUTY' and @Schema='dbo']
                Server[@Name='DB-SVR']/Database[@Name='SY']/StoredProcedure[@Name='APP_DCZPR_BID_REQUEST_INSERT' and @Schema='dbo']
                Server[@Name='DB-SVR']/Database[@Name='SY']/UserDefinedFunction[@Name='DAUFN_RETROINCOME' and @Schema='dbo']
                Server[@Name='DB-SVR']/Database[@Name='SY']/UserDefinedFunction[@Name='Comma2' and @Schema='dbo']
             */
            string urn = string.Format("Server[@Name='{0}']", rowData.Row["SVRNAME"]);
            urn += string.Format("/Database[@Name='{0}']", rowData.Row["DBNAME"]);
            if (("" + rowData.Row["type"]) == "U ")
            {
                urn += string.Format("/Table[@Name='{0}' and @Schema='{1}']", rowData.Row["name"], rowData.Row["schema"]);
            }
            else if (("" + rowData.Row["type"]) == "P ")
            {
                urn += string.Format("/StoredProcedure[@Name='{0}' and @Schema='{1}']", rowData.Row["name"], rowData.Row["schema"]);
            }
            else if (("" + rowData.Row["type"]) == "V ")
            {
                urn += string.Format("/View[@Name='{0}' and @Schema='{1}']", rowData.Row["name"], rowData.Row["schema"]);
            }
            else if (("" + rowData.Row["type"]) == "IF" || ("" + rowData.Row["type"]) == "TF" || ("" + rowData.Row["type"]) == "FN")
            {
                urn += string.Format("/UserDefinedFunction[@Name='{0}' and @Schema='{1}']", rowData.Row["name"], rowData.Row["schema"]);
            }
            else
                return; 

            //SynchronizeTree(urn);
            this.Sync<Control>(c => c.Enabled = false);
            Action<string> async = SynchronizeTree;
            async.BeginInvoke(urn, ir => async.EndInvoke(ir), null);
        }

        private void SynchronizeTree(string urn)
        {
            IObjectExplorerService objExplorer = ServiceProvider.GetService(typeof(IObjectExplorerService)) as IObjectExplorerService;
            INodeInformation f = objExplorer.FindNode(urn);
            //// 테스트 아직 로딩 안된 객체 선택! ( 찾아감 )
            Action<INodeInformation> infoAsync = objExplorer.SynchronizeTree;
            infoAsync.BeginInvoke(f, ir =>{
                try
                {
                    infoAsync.EndInvoke(ir);
                }
                finally
                {
                    this.Sync<Control>(c => c.Enabled = true);
                }
            }, null); 
        } 
        #endregion

        void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var cellColumn = dataGridView1.Columns[e.ColumnIndex];
            var rowData = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView;

            if (rowData != null)
            {
                string schema = "" + rowData.Row["schema"]; 
                string objectName = "" + rowData.Row["name"];
                string objectID = "" + rowData.Row["object_id"];
                string type = "" + rowData.Row["type"];
                if (cellColumn is DataGridViewButtonColumn && type != "U ")
                {
                    // sp를 가져와서!!! 수정편집창에 뿌려주기.
                    // object_definition( object_id )// scalar!
                    var conn = ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionGroupInfo;
                    if (conn.Count > 0)
                    {
                        if (conn[0].AdvancedOptions.AllKeys.Contains("DATABASE") &&
                            conn[0].AdvancedOptions.Count > 6)
                        {
                            string sqlConnectionString = string.Format(string.Format("Server={0};DataBase={3};UID={1};PWD={2};", conn[0].ServerName, conn[0].UserName, conn[0].Password, conn[0].AdvancedOptions["DATABASE"]));
                            string objectQuery = GetObjectDefinition(objectID);

                            string query = "";
                            using (System.Data.SqlClient.SqlConnection sCon = new System.Data.SqlClient.SqlConnection(sqlConnectionString))
                            {
                                using (System.Data.SqlClient.SqlCommand cmd = sCon.CreateCommand())
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = objectQuery;
                                    try
                                    {
                                        sCon.Open();
                                        query = "" + cmd.ExecuteScalar();
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    finally
                                    {
                                        sCon.Close();
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(query.Trim()) == false)
                            {
                                ServiceCache.ScriptFactory.CreateNewBlankScript(Microsoft.SqlServer.Management.UI.VSIntegration.Editors.ScriptType.Sql);
                                EnvDTE.TextDocument doc = (EnvDTE.TextDocument)ServiceCache.ExtensibilityModel.Application.ActiveDocument.Object(null);
                                doc.EndPoint.CreateEditPoint().Insert(query);
                            }
                        }
                    }
                }
                else
                { 
                    if (string.IsNullOrEmpty(( "" + objectName).Trim() ) == false)
                        Clipboard.SetText(objectName);
                }
            }
        }
         
        private string GetObjectDefinition(string objectID)
        {
            return "select object_definition( "+ objectID+" ) as [body]";
        }
         
        private void btnFind_Click(object sender, EventArgs e)
        {
            // todo : 찾기
            string find = cboFindText.Text.Trim().Trim("[]".ToArray());
            if (string.IsNullOrEmpty(find) == false)
            { 
                // sp돌려서... 가져오기. ( 비동기로 하면?? )
                QueryFind(find);
            }
        }

        private void QueryFind(string obj)
        {
            using (dataGridView1.DataSource as IDisposable)
            {
                dataGridView1.Columns.Clear();
            }
            dataGridView1.DataSource = null;

            Microsoft.SqlServer.Management.UI.VSIntegration.Editors.IScriptFactory scriptFactory = ServiceCache.ScriptFactory;
            Microsoft.SqlServer.Management.UI.VSIntegration.Editors.CurrentlyActiveWndConnectionInfo connectionIfno = scriptFactory.CurrentlyActiveWndConnectionInfo;
            Microsoft.SqlServer.Management.Smo.RegSvrEnum.UIConnectionInfo uConnectionInfo = connectionIfno.UIConnectionInfo;
            string connet = string.Format("{0}::{1}", uConnectionInfo.ServerName, uConnectionInfo.AdvancedOptions["DATABASE"]);
             
            // VSIntergration.dll삭제,  SqlPackageBase.dll이 4.0 런타임 이므로 프로젝트 자체를 4.0으로 변경.
            var conn = uConnectionInfo;//  ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionGroupInfo;
            //if (conn.Count > 0)
            //{
            //    if (conn[0].AdvancedOptions.AllKeys.Contains("DATABASE") &&
            //        conn[0].AdvancedOptions.Count > 6)
            //    { 
            //CreatedWindow.Caption = string.Format(string.Format("S:{0},U:{1},P:{2},D:{3}", conn[0].ServerName, conn[0].UserName, conn[0].Password, conn[0].AdvancedOptions[6]));
            string sqlConnectionString = string.Format(string.Format("Server={0};DataBase={3};UID={1};PWD={2};", conn.ServerName, conn.UserName, conn.Password, conn.AdvancedOptions["DATABASE"]));
            // AuthenticationType = 0 : Windows 인증
            // AuthenticationType = 1 : SQL Server Authentication
            if (conn.AuthenticationType != 1)
            {
                //Integrated Security, 기본값 false
                //         : false 이면 SQL Server 인증방식을 사용하여 연결시에 사용자의 ID 와 암호를 지정하여야 한다. 
                //         : true  이면 Windows 인증을 사용한다
                sqlConnectionString = $"Persist Security Info={conn.PersistPassword};Integrated Security=True;Initial Catalog={conn.AdvancedOptions["DATABASE"]};Server={conn.ServerName}";
            }

            string objectQuery = GetObjectQuery(obj);
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter(objectQuery, sqlConnectionString))
            {
                try
                {
                    adp.Fill(ds);

                    int idx = cboFindText.Items.IndexOf(obj);

                    if (0 <= idx)
                    {
                        cboFindText.Items.RemoveAt(idx);
                    }

                    cboFindText.Items.Insert(0, obj);
                    if (30 < cboFindText.Items.Count)
                    {
                        cboFindText.Items.RemoveAt(cboFindText.Items.Count - 1);
                    }

                    Settings.Default.FindTextHistory.Clear();
                    foreach (string s in cboFindText.Items)
                    {
                        Settings.Default.FindTextHistory.Add(s);
                    }
                    Settings.Default.Save();
                    cboFindText.Text = obj;
                }
                catch (Exception exx)
                {
                    return;
                }
            }

            DataGridViewTextBoxColumn ObjectNameColumn = new DataGridViewTextBoxColumn();
            ObjectNameColumn.Name = ObjectNameColumn.DataPropertyName = "name";
            ObjectNameColumn.ReadOnly = true;
            ObjectNameColumn.ValueType = typeof(string);
            ObjectNameColumn.Width = 160;
            ObjectNameColumn.HeaderText = "이름";
            dataGridView1.Columns.Add(ObjectNameColumn);

            DataGridViewTextBoxColumn TypeDescColumn = new DataGridViewTextBoxColumn();
            TypeDescColumn.Name = TypeDescColumn.DataPropertyName = "type_desc";
            TypeDescColumn.ReadOnly = true;
            TypeDescColumn.ValueType = typeof(string);
            TypeDescColumn.Visible = false;
            dataGridView1.Columns.Add(TypeDescColumn);

            DataGridViewButtonColumn ModifyButtonColumn = new DataGridViewButtonColumn();
            ModifyButtonColumn.Name = ModifyButtonColumn.DataPropertyName = "btn";
            ModifyButtonColumn.ReadOnly = true;
            ModifyButtonColumn.ValueType = typeof(string);
            ModifyButtonColumn.Width = 40;
            ModifyButtonColumn.HeaderText = "";
            dataGridView1.Columns.Add(ModifyButtonColumn);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].TableName;
            dataGridView1.ReadOnly = true;
            //    }
            //}
        }

        private string GetObjectQuery(string obj)
        {
            string qry = ""; 
            qry += @" 
select object_id, name, SCHEMA_NAME( schema_id ) as [schema], [type], type_desc, modify_date, case when [type] = 'U' then '복사' else '수정' end as btn, 
       @@SERVERNAME as SVRNAME, db_NAME() as DBNAME
from sys.objects
where name like N'%'+ '" + obj + @"' +'%'
and   type in ( 'U', 'P', 'V', 'FN', 'IF', 'TF' )
order by case type when 'U' then 1
                   when 'P' then 2
                   when 'V' then 3
                   when 'FN' then 4
                   when 'IF' then 4
                   when 'TF' then 4
                   else 90 
         end,
         name
";
            return qry;
        }

        //static ObjectFindForm CreatedFindForm = null;
        // static Window CreatedWindow = null;
        //internal static ObjectFindForm CreateWindow(DTE2 _applicationObject, AddIn _addInInstance, string currentObjectInfo )
        //{
        //    if (CreatedWindow == null)
        //    {
        //        object FindForm = null;
        //        Windows2 toolWins = _applicationObject.Windows as Windows2;
        //        CreatedWindow = toolWins.CreateToolWindow2(
        //                            _addInInstance,
        //                            Assembly.GetExecutingAssembly().Location,
        //                            typeof(ObjectFindForm).FullName,
        //                            "검색",
        //                            ObjectFindGUID,
        //                            ref FindForm);
        //        CreatedWindow.Visible = true;

        //        ObjectFindForm form = FindForm as ObjectFindForm;
        //        CreatedFindForm = form;
        //        form.SetApplicationInstance( _applicationObject );
        //        form.Dock = DockStyle.Fill; 
        //    }
        //    else
        //    {
        //        CreatedWindow.Visible = true;
        //    }

        //    ObjectFindForm oForm = CreatedFindForm as ObjectFindForm;
        //    if (oForm != null) oForm.SetFindObject(currentObjectInfo); 
        //    return CreatedFindForm;
        //} 

        //private void SetApplicationInstance(DTE2 _applicationObject)
        //{
        //    applicationObject = _applicationObject;            
        //}

        internal void SetVSPackage(VSPackage _package)
        {
            if (_package != null)
            {
                package = _package;
                applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
            }
        }

        internal void SetFindObject(string currentSelectedQuery)
        {
            Settings.Default.Reload();
            if (Settings.Default.FindTextHistory == null)
            {
                Settings.Default.FindTextHistory = new System.Collections.Specialized.StringCollection();
                Settings.Default.Save();
            }
            cboFindText.Items.Clear();
            foreach (string s in Settings.Default.FindTextHistory)
            {
                cboFindText.Items.Add(s);
            }
            cboFindText.Text = currentSelectedQuery.Trim();
            if (string.IsNullOrEmpty(currentSelectedQuery.Trim()) == false)
                btnFind.PerformClick();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                btnFind.PerformClick();
            }
        }

        private void btnColumnInfo_Click(object sender, EventArgs e)
        {
            if (applicationObject == null) return;

            TextDocument textDoc = (TextDocument)applicationObject.ActiveDocument.ActiveWindow.Document.Object("TextDocument");
            if (textDoc != null)
            {
                textDoc.StartPoint.CreateEditPoint();
                string currentSelectedQuery = GET_SP_JSFW_TABLE_COLUMN_INFO();
                textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);
            }
        }

        private void btnTableSelect_Click(object sender, EventArgs e)
        { 
            if (applicationObject == null) return;

            TextDocument textDoc = (TextDocument)applicationObject.ActiveDocument.ActiveWindow.Document.Object("TextDocument");
            if (textDoc != null)
            {
                textDoc.StartPoint.CreateEditPoint(); 
                string currentSelectedQuery = GET_SP_JS_TABLE_SELECT();
                textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);
               
            }
        }

        private void btnHelpTxt_Click(object sender, EventArgs e)
        {
            if (applicationObject == null) return;

            TextDocument textDoc = (TextDocument)applicationObject.ActiveDocument.ActiveWindow.Document.Object("TextDocument");
            if (textDoc != null)
            {
                textDoc.StartPoint.CreateEditPoint();
                string currentSelectedQuery = GET_QUERY_SP_HELPTXT();
                textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);

            }
        }

        private void 조회프로시져ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // todo : 조회프로시져!
            if (applicationObject == null) return;

            if (dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.RowIndex < 0 || dataGridView1.CurrentCell.ColumnIndex < 0) return;
            if (dataGridView1.CurrentRow == null) return;
            var rowData = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if (rowData != null)
            {
                string schema = "" + rowData.Row["schema"];
                string objectName = "" + rowData.Row["name"];
                string type = "" + rowData.Row["type"];
                if (type == "U ")
                {
                    TextDocument textDoc = (TextDocument)applicationObject.ActiveDocument.ActiveWindow.Document.Object("TextDocument");
                    if (textDoc != null)
                    {
                        textDoc.StartPoint.CreateEditPoint();
                        string currentSelectedQuery = GET_Query_R(objectName, "");
                        textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    }
                }
            }
        }

        private void cUD프로시져ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // todo : 등록, 수정, 삭제 프로시져! 
            if (applicationObject == null) return;

            if (dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.RowIndex < 0 || dataGridView1.CurrentCell.ColumnIndex < 0) return;
            if (dataGridView1.CurrentRow == null) return; 
            var rowData = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if (rowData != null)
            {
                string schema = "" + rowData.Row["schema"];
                string objectName = "" + rowData.Row["name"];
                string type = "" + rowData.Row["type"];
                if (type == "U ")
                {
                    TextDocument textDoc = (TextDocument)applicationObject.ActiveDocument.ActiveWindow.Document.Object("TextDocument");
                    if (textDoc != null)
                    {
                        textDoc.StartPoint.CreateEditPoint();
                        string currentSelectedQuery = GET_Query_CUD(objectName);
                        textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    }
                }
            }
        }

        private void 복사ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 프로시져 명 복사.
            if (applicationObject == null) return;

            if (dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.RowIndex < 0 || dataGridView1.CurrentCell.ColumnIndex < 0) return;
            if (dataGridView1.CurrentRow == null) return;
            var rowData = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if (rowData != null)
            {
                string schema = "" + rowData.Row["schema"];
                string objectName = "" + rowData.Row["name"];
                string type = "" + rowData.Row["type"];
  
                if (string.IsNullOrEmpty(objectName.Trim()) == false) 
                {
                    
                    Clipboard.SetText(objectName);
                }
            }
        }
    }

    static class ControlSync
    { 
        public static void Sync<T>( this Control ctrl, Action<T> syncMethod) where T : Control
        {
            if (ctrl.InvokeRequired)
            {
                ctrl?.Invoke(syncMethod, ctrl);
            }
            else
                syncMethod(ctrl as T);
        }
    
    }
}
