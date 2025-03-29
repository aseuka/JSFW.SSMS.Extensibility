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
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.SqlParser.Metadata;
using JSFW.SSMS.Extensibility.Controls;
using System.Collections;

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

                    조회프로시져ToolStripMenuItem.Visible = false;
                    cUD프로시져ToolStripMenuItem.Visible = false;
                    복사ToolStripMenuItem.Visible = false;
                    컬럼정보보기ToolStripMenuItem.Visible = true;

                    contextMenuStrip1.Show(dataGridView1, pt);
                }
                else if (type == "SQL_STORED_PROCEDURE")
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];
                    Point pt = dataGridView1.GetCellDisplayRectangle(2, e.RowIndex, false).Location;

                    조회프로시져ToolStripMenuItem.Visible = false;
                    cUD프로시져ToolStripMenuItem.Visible = false;
                    복사ToolStripMenuItem.Visible = false;
                    컬럼정보보기ToolStripMenuItem.Visible = false;

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
            try
            {
                IObjectExplorerService objExplorer = ServiceProvider.GetService(typeof(IObjectExplorerService)) as IObjectExplorerService;
                INodeInformation f = objExplorer.FindNode(urn);
                //// 테스트 아직 로딩 안된 객체 선택! ( 찾아감 )
                Action<INodeInformation> infoAsync = objExplorer.SynchronizeTree;
                infoAsync.BeginInvoke(f, ir =>
                {
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
            catch ( Exception ex) {
            }
            finally{
            }
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
                if (cellColumn is DataGridViewButtonColumn)
                {
                    string sqlConnectionString = GetDataBaseSqlConnectionString();

                    if (type != "U ")
                    {
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
                    else if (type == "U ")
                    {
                        ShowTableInfomationViewForm(objectID, objectName);
                    }                
                }
                else
                {
                    if (string.IsNullOrEmpty(("" + objectName).Trim()) == false)
                        Clipboard.SetText(objectName);
                }
            }
        }

        private string GetObjectDefinition(string objectID)
        {
            return "select object_definition( " + objectID + " ) as [body]";
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
            try
            {
                using (dataGridView1.DataSource as IDisposable)
                {
                    dataGridView1.Columns.Clear();
                }
                dataGridView1.DataSource = null;

                string sqlConnectionString = GetDataBaseSqlConnectionString();

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

                        SSMS.Extensibility.Properties.Settings.Default.FindTextHistory.Clear();
                        foreach (string s in cboFindText.Items)
                        {
                            SSMS.Extensibility.Properties.Settings.Default.FindTextHistory.Add(s);
                        }
                        SSMS.Extensibility.Properties.Settings.Default.Save();
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
            catch (Exception exxx)
            {

            }
        }

        private static string GetDataBaseSqlConnectionString()
        {
            string sqlConnectionString;
            Microsoft.SqlServer.Management.UI.VSIntegration.Editors.IScriptFactory scriptFactory = ServiceCache.ScriptFactory;
            Microsoft.SqlServer.Management.UI.VSIntegration.Editors.CurrentlyActiveWndConnectionInfo connectionIfno = scriptFactory.CurrentlyActiveWndConnectionInfo;
            Microsoft.SqlServer.Management.Smo.RegSvrEnum.UIConnectionInfo uConnectionInfo = connectionIfno.UIConnectionInfo;

            //ServerConnection serverConnection = new ServerConnection();
            //Server server = new Server(serverConnection);
            //string serverName = server.Name;
            //string databaseName = serverConnection.DatabaseName;
            //string userID = serverConnection.Login;
            //string password = serverConnection.Password;

            //string authMode = server.Settings.LoginMode.ToString();

            //if (authMode == "WindowsAuthentication")
            //{

            //}
            //string connet = string.Format("{0}::{1}", serverName, databaseName);

            // VSIntergration.dll삭제,  SqlPackageBase.dll이 4.0 런타임 이므로 프로젝트 자체를 4.0으로 변경.
            var conn = uConnectionInfo;//  ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionGroupInfo;
                                       //if (conn.Count > 0)
                                       //{
                                       //    if (conn[0].AdvancedOptions.AllKeys.Contains("DATABASE") &&
                                       //        conn[0].AdvancedOptions.Count > 6)
                                       //    { 
                                       //CreatedWindow.Caption = string.Format(string.Format("S:{0},U:{1},P:{2},D:{3}", conn[0].ServerName, conn[0].UserName, conn[0].Password, conn[0].AdvancedOptions[6]));
                                       //string sqlConnectionString = string.Format(string.Format("Server={0};DataBase={3};UID={1};PWD={2};", serverName, userID, password, databaseName));

            sqlConnectionString = string.Format(string.Format("Server={0};DataBase={1};UID={2};PWD={3};",
                conn.ServerName, conn.AdvancedOptions["DATABASE"], conn.UserName, conn.Password));

            // AuthenticationType = 0 : Windows 인증
            // AuthenticationType = 1 : SQL Server Authentication
            if (conn.AuthenticationType != 1)
            {
                //Integrated Security, 기본값 false
                //         : false 이면 SQL Server 인증방식을 사용하여 연결시에 사용자의 ID 와 암호를 지정하여야 한다. 
                //         : true  이면 Windows 인증을 사용한다
                sqlConnectionString = $"Persist Security Info={conn.PersistPassword};Integrated Security=True;Initial Catalog={conn.AdvancedOptions["DATABASE"]};Server={conn.ServerName}";
            }

            return sqlConnectionString;
        }

        private string GetObjectQuery(string obj)
        {
            string qry = "";
            qry += @" 
select object_id, name, SCHEMA_NAME( schema_id ) as [schema], [type], type_desc, modify_date, case when [type] = 'U' then '보기' else '수정' end as btn, 
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
            SSMS.Extensibility.Properties.Settings.Default.Reload();
            if (SSMS.Extensibility.Properties.Settings.Default.FindTextHistory == null)
            {
                SSMS.Extensibility.Properties.Settings.Default.FindTextHistory = new System.Collections.Specialized.StringCollection();
                SSMS.Extensibility.Properties.Settings.Default.Save();
            }
            cboFindText.Items.Clear();
            foreach (string s in SSMS.Extensibility.Properties.Settings.Default.FindTextHistory)
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


        ColumnInfomationViewForm tableInfomationViewForm = null;

        private void 컬럼정보보기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 컬럼 쿼리 후!!!
            if (applicationObject == null) return;

            if (dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.RowIndex < 0 || dataGridView1.CurrentCell.ColumnIndex < 0) return;
            if (dataGridView1.CurrentRow == null) return;
            var rowData = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if (rowData != null)
            {
                string schema = "" + rowData.Row["schema"];
                string objectID = "" + rowData.Row["object_id"];
                string objectName = "" + rowData.Row["name"];
                string type = "" + rowData.Row["type"];

                if (string.IsNullOrEmpty(objectName.Trim()) == false)
                {
                    //popup
                    ShowTableInfomationViewForm(objectID, objectName);
                }
            }
        }

        private void ShowTableInfomationViewForm(string objectID, string objectName)
        {
            try
            { 
                Microsoft.SqlServer.Management.UI.VSIntegration.Editors.IScriptFactory scriptFactory = ServiceCache.ScriptFactory;
                Microsoft.SqlServer.Management.UI.VSIntegration.Editors.CurrentlyActiveWndConnectionInfo connectionIfno = scriptFactory.CurrentlyActiveWndConnectionInfo;
                Microsoft.SqlServer.Management.Smo.RegSvrEnum.UIConnectionInfo uConnectionInfo = connectionIfno.UIConnectionInfo;

                //ServerConnection serverConnection = new ServerConnection();
                //Server server = new Server(serverConnection);
                //string serverName = server.Name;
                //string databaseName = serverConnection.DatabaseName;
                //string userID = serverConnection.Login;
                //string password = serverConnection.Password;

                //string authMode = server.Settings.LoginMode.ToString();

                //if (authMode == "WindowsAuthentication")
                //{

                //}
                //string connet = string.Format("{0}::{1}", serverName, databaseName);

                // VSIntergration.dll삭제,  SqlPackageBase.dll이 4.0 런타임 이므로 프로젝트 자체를 4.0으로 변경.
                var conn = uConnectionInfo;//  ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionGroupInfo;
                                           //if (conn.Count > 0)
                                           //{
                                           //    if (conn[0].AdvancedOptions.AllKeys.Contains("DATABASE") &&
                                           //        conn[0].AdvancedOptions.Count > 6)
                                           //    { 
                                           //CreatedWindow.Caption = string.Format(string.Format("S:{0},U:{1},P:{2},D:{3}", conn[0].ServerName, conn[0].UserName, conn[0].Password, conn[0].AdvancedOptions[6]));
                                           //string sqlConnectionString = string.Format(string.Format("Server={0};DataBase={3};UID={1};PWD={2};", serverName, userID, password, databaseName));

                string sqlConnectionString = string.Format(string.Format("Server={0};DataBase={1};UID={2};PWD={3};",
                    conn.ServerName, conn.AdvancedOptions["DATABASE"], conn.UserName, conn.Password));

                // AuthenticationType = 0 : Windows 인증
                // AuthenticationType = 1 : SQL Server Authentication
                if (conn.AuthenticationType != 1)
                {
                    //Integrated Security, 기본값 false
                    //         : false 이면 SQL Server 인증방식을 사용하여 연결시에 사용자의 ID 와 암호를 지정하여야 한다. 
                    //         : true  이면 Windows 인증을 사용한다
                    sqlConnectionString = $"Persist Security Info={conn.PersistPassword};Integrated Security=True;Initial Catalog={conn.AdvancedOptions["DATABASE"]};Server={conn.ServerName}";
                }

                string objectQuery = GetColumnQuery(objectID);
                DataSet ds = new DataSet();
                using (System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter(objectQuery, sqlConnectionString))
                {
                    try
                    {
                        adp.Fill(ds); 
                    }
                    catch (Exception exx)
                    {
                        return;
                    }
                }

                if (tableInfomationViewForm == null)
                {
                    tableInfomationViewForm = new ColumnInfomationViewForm();
                    tableInfomationViewForm.FormClosed += (ss, ee) => {
                        tableInfomationViewForm = null;
                    };
                }
                tableInfomationViewForm.StartPosition = FormStartPosition.CenterScreen;
                tableInfomationViewForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                tableInfomationViewForm.ShowInTaskbar = false;                
                tableInfomationViewForm.DataBind(objectID, objectName, ds, sqlConnectionString);
                tableInfomationViewForm.WindowState = FormWindowState.Normal;
                tableInfomationViewForm.Show(this);                
                tableInfomationViewForm.BringToFront();
            }
            catch (Exception exxx)
            {

            }
        }

        private static string GetColumnQuery(string objectID = "", string schemaName = "dbo")
        {
            return $@"
SELECT	DISTINCT
		C.COLID,
        C.NAME AS [NAME],
		TYPE_NAME( C.XUSERTYPE)+  CASE TYPE_NAME( C.XUSERTYPE )
										WHEN 'NUMERIC'  THEN '(' + CAST (C.XPREC AS VARCHAR) +','+ CAST(C.XSCALE AS VARCHAR) + ')'
										WHEN 'DECIMAL'  THEN '(' + CAST (C.XPREC AS VARCHAR) +','+ CAST(C.XSCALE AS VARCHAR) + ')'
										WHEN 'NVARCHAR' THEN '(' + IIF( C.[LENGTH] = -1, 'MAX', CAST (C.[LENGTH] AS VARCHAR))  + ')'
										WHEN 'VARCHAR'  THEN '(' + IIF( C.[LENGTH] = -1, 'MAX', CAST (C.[LENGTH] AS VARCHAR))  + ')'
										WHEN 'CHAR'		THEN '(' + CAST (C.[LENGTH] AS VARCHAR)  + ')'
										ELSE ''
									END AS [TYPE_NAME],
		ISNULL( P.VALUE , '') AS [DESC],
		CASE WHEN C.ISNULLABLE = 0
			THEN CASE WHEN ISNULL(I.INDEX_COLUMN_ID, 0) <> 0 THEN 'N.N / PK' ELSE 'N.N' END
				+ CASE WHEN ISNULL(K.INDEX_COLUMN_ID, 0) <> 0 THEN ' / IX' ELSE '' END
				+ ISNULL(CASE J.IS_IDENTITY WHEN 1 THEN ' / IDT(' + CAST(SEED_VALUE AS VARCHAR) + ',' + CAST(INCREMENT_VALUE AS VARCHAR) + ')' ELSE '' END, '')
			ELSE ''
		END + CASE WHEN ISNULL(L.INDEX_COLUMN_ID, 0) <> 0 THEN 'UK' ELSE '' END AS [NULL_KEY_TYPE],
		ISNULL(REPLACE(C.COLLATION, 'KOREAN_WANSUNG_', ''), '') AS [COLLATION] 
FROM SYSCOLUMNS C
	LEFT OUTER JOIN SYS.EXTENDED_PROPERTIES P
		ON	P.MAJOR_ID	= C.ID
		AND P.MINOR_ID	= C.COLID
		AND P.CLASS		= 1	-- OBJECT_OR_COLUMN
		AND P.NAME		= 'MS_DESCRIPTION'
	LEFT OUTER JOIN SYS.EXTENDED_PROPERTIES Q
		ON	Q.MAJOR_ID	= C.ID
		AND Q.MINOR_ID	= 0	-- TABLE
		AND Q.NAME		= 'MS_DESCRIPTION'
	LEFT OUTER JOIN SYS.INDEX_COLUMNS I		-- PK
		ON	I.OBJECT_ID	= C.ID
		AND I.COLUMN_ID	= C.COLID
		AND I.INDEX_ID = (SELECT INDEX_ID FROM SYS.INDEXES WHERE OBJECT_ID = C.ID AND IS_PRIMARY_KEY = 1)	-- PRIMARY KEY
	LEFT OUTER JOIN SYS.IDENTITY_COLUMNS J	-- IDENTITY
		ON	J.OBJECT_ID = C.ID
		AND J.COLUMN_ID = C.COLID
	LEFT OUTER JOIN SYS.INDEX_COLUMNS K		-- INDEX
		ON	K.OBJECT_ID	= C.ID
		AND K.COLUMN_ID	= C.COLID
		AND K.INDEX_ID IN (SELECT INDEX_ID FROM SYS.INDEXES WHERE OBJECT_ID = C.ID AND IS_PRIMARY_KEY = 0 AND IS_UNIQUE_CONSTRAINT = 0 )	-- INDEX KEY
	LEFT OUTER JOIN SYS.INDEX_COLUMNS L	-- UNIQE KEY
		ON	L.OBJECT_ID	= C.ID
		AND L.COLUMN_ID	= C.COLID
		AND L.INDEX_ID IN (SELECT INDEX_ID FROM SYS.INDEXES WHERE OBJECT_ID = C.ID AND IS_UNIQUE = 1 AND IS_UNIQUE_CONSTRAINT = 1)	-- UNIQUE KEY
	LEFT OUTER JOIN SYS.DEFAULT_CONSTRAINTS M
		ON	M.PARENT_OBJECT_ID = C.ID
		AND M.PARENT_COLUMN_ID	= C.COLID
		AND M.[TYPE] = 'D'
WHERE C.ID = '{objectID.Trim()}' AND C.NUMBER = 0
ORDER BY C.COLID
";
        }

//        private string UpdateExtendedScript(string colName, string dbdesc, string txt, string schema = "dbo")
//        {
//            /*
//EXEC SP_ADDEXTENDEDPROPERTY 'MS_Description', @DESCRIPTION, 'user', @DB_USER, 'table', @TABLE_NAME, 'column', @COLUMN_NAME       -- 컬럼 정보 추가    
//EXEC SP_UPDATEEXTENDEDPROPERTY 'MS_Description', @DESCRIPTION, 'user', @DB_USER, 'table', @TABLE_NAME, 'column', @COLUMN_NAME    -- 컬럼 정보 수정

//EXEC SP_ADDEXTENDEDPROPERTY  @NAME=N'MS_DESCRIPTION', @VALUE=N'지급조건', @LEVEL0TYPE=N'SCHEMA', @LEVEL0NAME=N'CubeCon',  @LEVEL1TYPE=N'TABLE', @LEVEL1NAME=N'TSAP_VENDOR', @LEVEL2TYPE=N'COLUMN', @LEVEL2NAME=N'TERMS'

//             */
//            string script = "EXEC";
//            if (txt == "추가")
//            {
//                script += $@" SP_ADDEXTENDEDPROPERTY @NAME=N'MS_DESCRIPTION', @VALUE=N'{dbdesc.Trim()}', @LEVEL0TYPE=N'SCHEMA', @LEVEL0NAME=N'{schema}', @LEVEL1TYPE=N'TABLE', @LEVEL1NAME=N'{txtFindTbName.Text.Trim()}', @LEVEL2TYPE=N'COLUMN', @LEVEL2NAME=N'{colName.Trim()}'";
//            }
//            else
//            {
//                script += $@" SP_UPDATEEXTENDEDPROPERTY @NAME=N'MS_DESCRIPTION', @VALUE=N'{dbdesc.Trim()}', @LEVEL0TYPE=N'SCHEMA', @LEVEL0NAME=N'{schema}', @LEVEL1TYPE=N'TABLE', @LEVEL1NAME=N'{txtFindTbName.Text.Trim()}', @LEVEL2TYPE=N'COLUMN', @LEVEL2NAME=N'{colName.Trim()}'";
//            }
//            return script;
//        }
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
