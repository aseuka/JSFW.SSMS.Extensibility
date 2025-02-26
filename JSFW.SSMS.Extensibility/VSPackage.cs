using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.VisualStudio.CommandBars;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.UI.Grid;
using System.IO;
using System.Drawing;
//C:\Program Files (x86)\Microsoft SQL Server Management Studio 18\Common7\IDE\Ssms.exe

//C:\Program Files (x86)\Microsoft SQL Server\140\Tools\Binn\ManagementStudio\Ssms.exe
//xcopy "$(TargetPath)" "C:\Program Files (x86)\Microsoft SQL Server\140\Tools\Binn\ManagementStudio\Extensions\JSFW.SSMS.Extensibility\$(TargetFileName)" /y /r

namespace JSFW.SSMS.Extensibility
{ 
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(VSPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class VSPackage : Package
    {
        //[ProvideAutoLoad(UIContextGuids.SolutionExists)]
        // 이 어트리뷰트가 있으면 ssms실행즉시 로딩된다.
        // 개발중에 시작시 이벤트가 안먹힐때가...  
        CommandBarButton HeaderFrozenColumnColumnButton { get; set; }


        internal static readonly string _DRIVE = GetDrive(@"D:\");
        private static string GetDrive(string drive)
        {
            if (!Directory.GetLogicalDrives().Contains(drive))
            {
                drive = Directory.GetLogicalDrives()[0];
            }
            return drive;
        }

        internal readonly static string _DIR_JSFW = _DRIVE + "JSFW\\";


        /// <summary>
        /// VSPackage GUID string. ((레지스트리 KEY로도 사용함.))
        /// </summary>
        public const string PackageGuidString = "e499b659-abb0-4651-a054-3deb4f5b6541";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSPackage"/> class.
        /// </summary>
        public VSPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private System.IServiceProvider ServiceProvider
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize(); 
             
            JSFW.SSMS.Extensibility.Cmds.ColumnAlign.Initialize(this);
            JSFW.SSMS.Extensibility.Cmds.ObjectFinder.Initialize(this);            
            IncludeCommands();
            DelayAddSkipLoadingReg();
        }
         
        private void AddSkipLoadingReg()
        {
            var myPackage = this.UserRegistryRoot.CreateSubKey(@"Packages\{" + VSPackage.PackageGuidString + "}");           
            myPackage.SetValue("SkipLoading", 1);
        }

        private void DelayAddSkipLoadingReg()
        {
            var delay = new Timer(); 
            delay.Tick += delegate (object o, EventArgs e)
            {
                delay.Stop();
                AddSkipLoadingReg();
            };
            delay.Interval = 1000;
            delay.Start();
        }

        private void IncludeCommands()
        {
            var _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;

            // TextEdit에서 컨텍스트 메뉴가 떠야 할때!   
            //CommandBar vsBarTextEditorBar = ((CommandBars)_applicationObject.CommandBars)["SQL Files Editor Context"];
            //CommandBarControl HeaderColumnsParsing = vsBarTextEditorBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            //var HeaderColumnsParsingButton = (CommandBarButton)HeaderColumnsParsing;
            //HeaderColumnsParsingButton.Visible = false;
            //HeaderColumnsParsingButton.Enabled = true;
            //HeaderColumnsParsingButton.Caption = "Header 복사(JSFW : Q+Q After)";
            //HeaderColumnsParsingButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            //HeaderColumnsParsingButton.Click += HeaderColumnsParsingButton_Click;

            // TextEdit에서 컨텍스트 메뉴가 떠야 할때!   
            CommandBar vsBarTextEditorBar = ((CommandBars)_applicationObject.CommandBars)["SQL Files Editor Context"];
            CommandBarControl CodeTemplate = vsBarTextEditorBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            var CodeTemplateButton = (CommandBarButton)CodeTemplate;
            CodeTemplateButton.Visible = true;
            CodeTemplateButton.Enabled = true;
            CodeTemplateButton.Caption = "쿼리 저장소";
            CodeTemplateButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            CodeTemplateButton.Click += CodeTemplateButton_Click;

            CommandBarControl CodeBackup = vsBarTextEditorBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            var CodeBackupButton = (CommandBarButton)CodeBackup;
            CodeBackupButton.Visible = true;
            CodeBackupButton.Enabled = true;
            CodeBackupButton.Caption = "백업(임시저장)";
            CodeBackupButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            CodeBackupButton.Click += CodeBackupButton_Click;
             
            // 결과 그리드관련
            CommandBar gridContextCommandBar = ((CommandBars)_applicationObject.CommandBars)["SQL Results Grid Tab Context"];
            CommandBarControl HeaderColumnsAlignCopy = gridContextCommandBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            var HeaderColumnsAlignCopyButton = (CommandBarButton)HeaderColumnsAlignCopy;
            HeaderColumnsAlignCopyButton.Visible = true;
            HeaderColumnsAlignCopyButton.Enabled = true;
            HeaderColumnsAlignCopyButton.Caption = "Header 복사(JSFW)";
            HeaderColumnsAlignCopyButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            HeaderColumnsAlignCopyButton.Click += HeaderColumnsAlignCopyButton_Click;

            CommandBarControl HeaderFrozenColumn = gridContextCommandBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            HeaderFrozenColumnColumnButton = (CommandBarButton)HeaderFrozenColumn;
            HeaderFrozenColumnColumnButton.Visible = true;
            HeaderFrozenColumnColumnButton.Enabled = true;
            HeaderFrozenColumnColumnButton.Caption = "틀고정";
            HeaderFrozenColumnColumnButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            HeaderFrozenColumnColumnButton.Click += HeaderFrozenColumnColumnButton_Click;

            CommandBarControl HeaderColumnFinding = gridContextCommandBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            var HeaderColumnFindingButton = (CommandBarButton)HeaderColumnFinding;
            HeaderColumnFindingButton.Visible = true;
            HeaderColumnFindingButton.Enabled = true;
            HeaderColumnFindingButton.Caption = "컬럼찾기";
            HeaderColumnFindingButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            HeaderColumnFindingButton.Click += HeaderColumnFindingButton_Click;

            CommandBarControl ConditionCreate = gridContextCommandBar.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            var ConditionCreateButton = (CommandBarButton)ConditionCreate;
            ConditionCreateButton.Visible = true;
            ConditionCreateButton.Enabled = true;
            ConditionCreateButton.Caption = "조건문 생성";
            ConditionCreateButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
            ConditionCreateButton.Click += ConditionCreateButton_Click;

            // 쿼리 실행 전 > 이벤트! ( 툴바에 [실행!] 버튼 명령관련!!
            //if (_applicationObject.Events.CommandEvents["{52692960-56BC-4989-B5D3-94C47A513E8D}", 1] != null)
            //{
            //    //    _applicationObject.Events.CommandEvents.BeforeExecute += VSPackage_BeforeExecute;
            //    //    _applicationObject.Events.CommandEvents.AfterExecute += VSPackage_AfterExecute;
            //}
            // 커맨드 종류 알아내는 방법.
            //_applicationObject.Events.DocumentEvents.DocumentOpened += DocumentEvents_DocumentOpened;
            ResetKeyBinding_QuickLunch();
        }
         
        private void ResetKeyBinding_QuickLunch()
        {
            /*
              ## 2017에서 빠른실행 (Ctrl + Q )가 포커스를 땡겨가서 포맷터가 단축키로 활용이 안되어 재설정.
             
              # 제거 
                 Window.ActivateQuickLaunch
              # 다시 설정
                 Name=Tools.ColumnAlign, ID=256, Guid={1682DDCE-F8B8-4F00-94A9-00CF439B8458}, Key=
                 Name=Tools.개체찾기, ID=4129, Guid={1682DDCE-F8B8-4F00-94A9-00CF439B8458}, Key=
                 Name=Tools.FormatterSetting, ID=4130, Guid={1682DDCE-F8B8-4F00-94A9-00CF439B8458}, Key=
                 Name=Tools.Formatter, ID=4131, Guid={1682DDCE-F8B8-4F00-94A9-00CF439B8458}, Key=
            */

            var _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2; 
            foreach (EnvDTE.Command cmd in _applicationObject.Commands)
            {
                switch (cmd.Name)
                {
                    default: break;

                    case "Window.ActivateQuickLaunch": // 빠른실행 
                        cmd.Bindings = new object[0] { }; // 제거
                        break;
                    case "Tools.ColumnAlign":
                        //cmd.Bindings = new object[1] { "전역::CTRL+Q,CTRL+R" };
                        cmd.Bindings = new object[0] { };
                        break;
                    case "Tools.개체찾기":
                        cmd.Bindings = new object[1] { "전역::CTRL+Q,CTRL+F" };
                        break;                    
                } 
            }
        }

        private void CodeBackupButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            string txt = GetSelection(); 
            File.WriteAllText(CreateCodeTmpForm.Root + "백업.txt", txt, Encoding.UTF8);
        }

        private void CodeTemplateButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            ShowToolWindow();
        }

        


        private IVsWindowFrame m_windowFrame = null;
        JSFW.Controls.CodeTemplateListUserControl codeTemplateListCtrl = null;

        private void ShowToolWindow()
        {
            const string TOOLWINDOW_GUID = "C53FC666-B883-4788-9C3C-D60799BC7852";

            if (codeTemplateListCtrl == null) codeTemplateListCtrl = new JSFW.Controls.CodeTemplateListUserControl();

            codeTemplateListCtrl.ListRefresh();
            codeTemplateListCtrl.ExportToString -= SnippetListCtrl_ExportToString;
            codeTemplateListCtrl.ExportToString += SnippetListCtrl_ExportToString;

            codeTemplateListCtrl.ImportToString -= GetSelection;
            codeTemplateListCtrl.ImportToString += GetSelection;

            if (m_windowFrame == null)
            {
                m_windowFrame = CreateToolWindow("코드변환 템플릿 목록", TOOLWINDOW_GUID, codeTemplateListCtrl);

                // TODO: Initialize m_userControl if required adding a method like:
                //    internal void Initialize(VSPackageToolWindowPackage package)
                // and pass this instance of the package:
                //    m_userControl.Initialize(this);
            }

            // 읽어온 ... 값 snippetListControl에 ... 전달?
            m_windowFrame.Show();
        }

        private void SnippetListCtrl_ExportToString(string txt)
        {
            if (!string.IsNullOrEmpty((txt ?? "").Trim()))
            {
                SetSelection(txt);
            }
        }

        private IVsWindowFrame CreateToolWindow(string caption, string guid, JSFW.Controls.CodeTemplateListUserControl snippet)
        {
            const int TOOL_WINDOW_INSTANCE_ID = 0; // Single-instance toolwindow

            IVsUIShell uiShell;
            Guid toolWindowPersistenceGuid;
            Guid guidNull = Guid.Empty;
            int[] position = new int[1];
            int result;
            IVsWindowFrame windowFrame = null;
            uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
            toolWindowPersistenceGuid = new Guid(guid);
            result = uiShell.CreateToolWindow((uint)__VSCREATETOOLWIN.CTW_fInitNew,
                  TOOL_WINDOW_INSTANCE_ID, snippet, ref guidNull, ref toolWindowPersistenceGuid,
                  ref guidNull, null, caption, position, out windowFrame);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(result);
            return windowFrame;
        }

        private string GetSelection()
        {
            string setting = "";

            EnvDTE80.DTE2 _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
            //Check active document
            if (_applicationObject.ActiveDocument != null)
            {
                //Get active document
                EnvDTE.TextDocument objTextDocument = (EnvDTE.TextDocument)_applicationObject.ActiveDocument.Object("");
                EnvDTE.TextSelection objTextSelection = objTextDocument.Selection;

                if (!String.IsNullOrEmpty(objTextSelection.Text))
                {
                    //Get selected text
                    setting = objTextSelection.Text;
                }
            }
            return setting;
        }

        private void SetSelection(string txt)
        {
            EnvDTE80.DTE2 _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;

            //Check active document
            if (_applicationObject.ActiveDocument != null)
            {
                //Get active document
                EnvDTE.TextDocument objTextDocument = (EnvDTE.TextDocument)_applicationObject.ActiveDocument.Object("");
                EnvDTE.TextSelection objTextSelection = objTextDocument.Selection;

                if (!String.IsNullOrEmpty(txt))
                {
                    objTextSelection.Insert(txt, (int)EnvDTE.vsInsertFlags.vsInsertFlagsContainNewText);
                    //  objTextDocument.Selection.Text = txt;
                }
            }
        }

        private void HeaderColumnsParsingButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Clipboard.Clear();
            var _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
            EnvDTE.TextDocument textDoc = (EnvDTE.TextDocument)_applicationObject.ActiveWindow.Document.Object("TextDocument");
            if (textDoc != null)
            {
                textDoc.StartPoint.CreateEditPoint();
                string currentSelectedQuery = textDoc.Selection.Text;

                //커맨드를 삽입하고
                if (string.IsNullOrEmpty(currentSelectedQuery) == false)
                {
                    List<string> lines = new List<string>();
                    Dictionary<string, int> DuplicateIDs = new Dictionary<string, int>();
                    foreach (var txt in currentSelectedQuery.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrEmpty(txt.Trim())) continue;
                        if (txt.Trim().StartsWith("--")) continue;

                        if (txt.Trim().IndexOf("--") > 0)
                        {
                            string desc = txt.Substring(txt.IndexOf("--")).Trim();
                            string id = GetID(txt.Substring(0, txt.IndexOf("--")).Trim());

                            if (DuplicateIDs.ContainsKey(id))
                            {
                                string oldID = id;
                                do
                                {
                                    id = oldID + GetColName(DuplicateIDs[oldID]);
                                    DuplicateIDs[oldID]++;
                                }
                                while (DuplicateIDs.ContainsKey(id));

                                if (DuplicateIDs.ContainsKey(id))
                                {
                                    DuplicateIDs[id]++;
                                }
                                else DuplicateIDs.Add(id, 0);
                            }
                            else DuplicateIDs.Add(id, 0);

                            lines.Add(string.Format("{0}{1}", id, desc).Replace("--", ":"));
                        }
                    }

                    if (lines.Count > 0)
                    {
                        Clipboard.SetText(string.Join(Environment.NewLine, lines.ToArray()));
                    }
                    //textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);
                } 
            }
        }
         
        private string GetID(string txt)
        {
            string id = "";

            for (int loop = txt.Length - 1; loop >= 0; loop--)
            {
                if (txt[loop] != ' ' &&
                    txt[loop] != '.' &&
                    txt[loop] != ')')
                {
                    id = txt[loop] + id;
                }
                else break;
            }
            return id.Trim("[], ".ToArray());
        }

        private string GetColName(int col)
        {
            // https://www.add-in-express.com/creating-addins-blog/2013/11/13/convert-excel-column-number-to-name/
            int div = col + 1;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        } 

        private void DocumentEvents_DocumentOpened(EnvDTE.Document Document)
        {
            //커맨드 종류 알아내기. 
            EnvDTE.TextDocument textDoc = (EnvDTE.TextDocument)Document.Object("TextDocument");
            if (textDoc != null)
            {
                var _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
                StringBuilder sb = new StringBuilder();
                string keys = "";
                foreach (EnvDTE.Command cmd in _applicationObject.Commands)
                {
                    keys = "";
                    if (cmd.Bindings != null && cmd.Bindings is Array && 0 < ((Array)cmd.Bindings).Length)
                    {
                        foreach (var bindItem in ((Array)cmd.Bindings))
                        {
                            keys += bindItem + " || ";
                        }
                    }
                    sb.AppendFormat(@"Name={0}, ID={1}, Guid={2}, Key={3}", cmd.Name, cmd.ID, cmd.Guid, keys);
                    sb.AppendLine();
                }
                textDoc.StartPoint.CreateEditPoint();
                textDoc.Selection.Insert("" + sb, (int)EnvDTE.vsInsertFlags.vsInsertFlagsContainNewText);
                textDoc = null;
            }
        }

        //private void VSPackage_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        //{
        //    // 추후 : 틀고정, 해제 문자열!! 처리하고 싶을때... 
        //    if (Guid == "{52692960-56BC-4989-B5D3-94C47A513E8D}" && ID == 1)
        //    { 
        //    }
        //}

        //private void VSPackage_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut)
        //{
        //    if (Guid == "{52692960-56BC-4989-B5D3-94C47A513E8D}" && ID == 1)
        //    {
        //    }
        //}

        private void HeaderFrozenColumnColumnButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Microsoft.SqlServer.Management.UI.Grid.GridControl grid = ShellUtilities.GetActiveGridControl() as Microsoft.SqlServer.Management.UI.Grid.GridControl;
            if (grid == null)
                return;

            if( 0 < grid.FirstScrollableColumn && grid.FirstScrollableColumn < grid.GridColumnsInfo.Count)
            grid.GridColumnsInfo[grid.FirstScrollableColumn].IsWithRightGridLine = false;
            grid.FirstScrollableColumn = 1;

            grid.Refresh();

            grid.Paint -= Grid_Paint;
            grid.Paint += Grid_Paint;

            long row = 0;
            int col = 1;
            grid.GetCurrentCell(out row, out col);
            if (col + 1 < grid.GridColumnsInfo.Count)
            {
                grid.FirstScrollableColumn = col + 1;
                grid.GridColumnsInfo[col + 1].IsWithRightGridLine = true;               
            }
            grid.Refresh();
        }

        System.Drawing.Pen FirstScrollableColumnRightLinePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 2f);
        private void Grid_Paint(object sender, PaintEventArgs e)
        {
            Microsoft.SqlServer.Management.UI.Grid.GridControl grid = sender as Microsoft.SqlServer.Management.UI.Grid.GridControl;
            if (1 < grid.FirstScrollableColumn)
            {
                int x = 0;

                for (int col = 0; col < grid.ColumnsNumber; col++)
                {
                    x += grid.GetColumnWidth(col);
                    if ((grid.FirstScrollableColumn - 1) <= col) break;
                }
                e.Graphics.DrawLine(FirstScrollableColumnRightLinePen, x + grid.FirstScrollableColumn, 0, x + grid.FirstScrollableColumn, grid.HeaderHeight);
            }
        }

        private void HeaderColumnFindingButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            // 그리드 컬럼 찾기.
            // -- Header 정보를 이용해서 - 그리드 Header를 설정하거나 데이타 상세 컨트롤을 만들어낼때 사용 함.
            IGridControl grid = ShellUtilities.GetActiveGridControl();
            if (grid == null)
                return;
            IGridStorage gs = grid.GridStorage;
            //string text = "";

            // 데이타 가져올때 소스
            //for (int col = 1; col < grid.ColumnsNumber; col++)
            //{
            //    text += gs.GetCellDataAsString(0, col) + ", "; 
            //}

            using (FindColumnNameForm fcn = new FindColumnNameForm(grid))
            {
                fcn.ShowDialog(grid as Control);
            }
        }

        private void ConditionCreateButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            // 조건문 생성하기.
            IGridControl grid = ShellUtilities.GetActiveGridControl();
            if (grid == null)
                return;
            IGridStorage gs = grid.GridStorage;
            Dictionary<string, List<string>> whereConditions = new Dictionary<string, List<string>>();
            GridHeader headers = GetNonPublicField(grid, "m_gridHeader") as GridHeader;
            if (headers == null) return;

            foreach (BlockOfCells cell in grid.SelectedCells)
            {
                // cell.X : col
                // cell.Y : row
                // cell.Right : end col
                // cell.Bottom : end row

                for (long row = cell.Y; row <= cell.Bottom; row++)
                {
                    for (int col = cell.X; col <= cell.Right; col++)
                    {
                        string column = headers[col].Text.Trim();
                        if (column.StartsWith("<"))
                        {
                            try
                            {
                                string[] cols = column.Split('(');
                                if (1 < cols.Length)
                                    column = cols[1].TrimEnd(")>".ToArray());
                            }
                            catch { }
                        }

                        if (whereConditions.ContainsKey(column) == false)
                            whereConditions.Add(column, new List<string>());

                        if (whereConditions[column].Contains(gs.GetCellDataAsString(row, col))) continue;

                        whereConditions[column].Add(gs.GetCellDataAsString(row, col));
                    }
                }
            }

            //데이타 가져올때 소스
            List<string> where = new List<string>();
            foreach (var item in whereConditions)
            {
                string w = "";
                if (item.Value.Count < 1) continue;

                if (1 < item.Value.Count)
                {
                    w = string.Format(" {0} in ( {1} ) ", item.Key, string.Join(",", item.Value.Select(s => "'" + s + "'").ToArray()));
                }
                else
                {   // 1개일때
                    w = item.Key + " = " + string.Join(",", item.Value.Select(s => "'" + s + "'").ToArray());
                }
                where.Add(w);
            }

            string condition = string.Join("and ", where.ToArray());

            if (string.IsNullOrEmpty(condition)) return;

            Clipboard.SetText(condition);
        }

        private object GetNonPublicField(object obj, string propertyName)
        {
            System.Reflection.FieldInfo p = obj.GetType().GetField(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
            return p.GetValue(obj);
        }
         
        private void HeaderColumnsAlignCopyButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            // todo : 쿼리 결과 그리드에 접근하여 - Header 정보를 가져옴.
            // -- Header 정보를 이용해서 - 그리드 Header를 설정하거나 데이타 상세 컨트롤을 만들어낼때 사용 함.
            IGridControl grid = ShellUtilities.GetActiveGridControl();
            if (grid == null)
                return;
            IGridStorage gs = grid.GridStorage;
            string text = "";

            // 데이타 가져올때 소스
            //for (int col = 1; col < grid.ColumnsNumber; col++)
            //{
            //    text += gs.GetCellDataAsString(0, col) + ", "; 
            //}

            // Header 가져오는 소스
            string hd = "";
            System.Drawing.Bitmap bmp;
            //grid.GetHeaderInfo(1, out hd, out bmp); 

            Dictionary<string, int> DuplicateIDs = new Dictionary<string, int>();

            for (int col = 1; col < grid.ColumnsNumber; col++)
            {
                grid.GetHeaderInfo(col, out hd, out bmp);

                if (string.IsNullOrEmpty(hd.Trim())) continue;

                if (hd.Trim() == "(열 이름 없음)")
                {
                    hd = "unknown";
                }

                if (DuplicateIDs.ContainsKey(hd.TrimEnd()))
                {
                    string oldHd = hd.TrimEnd();
                    do
                    {
                        hd = oldHd + GetColName(DuplicateIDs[oldHd]);
                        DuplicateIDs[oldHd]++;
                    } while (DuplicateIDs.ContainsKey(hd));

                    if (DuplicateIDs.ContainsKey(hd.TrimEnd()))
                    {
                        DuplicateIDs[hd.TrimEnd()]++;
                    }
                    else
                        DuplicateIDs.Add(hd.TrimEnd(), 0);
                }
                else DuplicateIDs.Add(hd.TrimEnd(), 0);

                text += $", {hd.TrimEnd()}{Environment.NewLine}";// + "\t";
            }

            //GridControl gc = grid as GridControl; 
            if (string.IsNullOrEmpty(text.Trim()) == false)
            {
                Clipboard.SetText(text.Trim());
            }
        }
        #endregion
    }
}
