using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JSFW.SSMS.Extensibility.Cmds
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ColumnAlign
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1682ddce-f8b8-4f00-94a9-00cf439b8458");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAlign"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ColumnAlign(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);                
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ColumnAlign Instance
        {
            get;
            private set;
        }

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
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ColumnAlign(package); 
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "ColumnAlign";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);


            try
            {
                var _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
                if (_applicationObject.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument)
                {
                    //열려진 문서의 인스턴스를 취득해서
                    TextDocument textDoc = (TextDocument)_applicationObject.ActiveWindow.Document.Object("TextDocument");
                    if (textDoc != null)
                    {
                        textDoc.StartPoint.CreateEditPoint();
                        string currentSelectedQuery = textDoc.Selection.Text;
                        //커맨드를 삽입하고
                        if (string.IsNullOrEmpty(currentSelectedQuery) == false)
                        {
                            currentSelectedQuery = Exec_ColumAlign(currentSelectedQuery);
                            textDoc.Selection.Insert(currentSelectedQuery, (int)vsInsertFlags.vsInsertFlagsContainNewText);
                        }
                        /*textDoc.DTE.ExecuteCommand("Query.Execute"); 실행할때  */

                        // VSIntergration.dll삭제,  SqlPackageBase.dll이 4.0 런타임 이므로 프로젝트 자체를 4.0으로 변경.
                        //var conn = ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionGroupInfo;

                        //textDoc.Selection.NewLine();
                        //textDoc.Selection.Insert(string.Format("S:{0},U:{1},P:{2},D:{3}", conn[0].ServerName, conn[0].UserName, conn[0].Password, conn[0].AdvancedOptions[6]), 0);

                        /*
                        // Nuget 패키지관리자 ( 콘솔 ) : Install-Package VSSDK.Shell.10 
                        // Shell.10.dll이 필요해서 구글링.
                        // : ServiceProvider 
                        */
                        IObjectExplorerService objExplorer = ServiceProvider.GetService(typeof(IObjectExplorerService)) as IObjectExplorerService;
                        int cnt = 0;
                        INodeInformation[] r = null;
                        objExplorer.GetSelectedNodes(out cnt, out r);

                        //////DB정보 : Server[@Name='DB-SVR']/Database[@Name='SY']
                        //////테이블 : Server[@Name='DB-SVR']/Database[@Name='SY']/Table[@Name='DZZT_CODE' and @Schema='dbo']
                        ////INodeInformation f = objExplorer.FindNode("Server[@Name='DB-SVR']/Database[@Name='SY']/Table[@Name='DZZT_CODE' and @Schema='dbo']");
                        ////// 테스트 아직 로딩 안된 객체 선택! ( 찾아감 )
                        ////objExplorer.SynchronizeTree(f);

                        // Like 검색!!은?
                        //  : 폼을 새로 만들어서 싱크거는게 좋을 듯함

                        //SendKeys.Send("^5");
                        //SendKeys.Flush();
                    }
                }
            }
            catch (Exception exx)
            {
                System.Windows.Forms.MessageBox.Show(exx.Message);
            }
        }
         
        private string Exec_ColumAlign(string currentSelectedQuery)
        {
            string[] columns = currentSelectedQuery.Trim().Replace(" ", "\t").Replace("\t", Environment.NewLine).Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            List<string> columnDatas = new List<string>();

            string dicFile = VSPackage._DIR_JSFW + "dic.txt";
            string diccontent = System.IO.File.ReadAllText(dicFile);
            string[] dics = diccontent.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            Dictionary<string, int> DuplicationKey = new Dictionary<string, int>();

            // 선택 : 1) ID:이름 (newline)
            //       2) ID (,)
            bool isAlignGubun = true;
            using (ColumnAlignChoiceForm fm = new ColumnAlignChoiceForm())
            {
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    isAlignGubun = fm.AlignGubun;
                }
                else return "";
            }

            foreach (string item in dics)
            {
                string cd = "";
                string ds = "";
                string[] items = item.Split('\t');
                if (items.Length > 0)
                {
                    cd = ds = items[0];
                }

                if (items.Length > 1)
                {
                    ds = items[1];
                }
                if (string.IsNullOrEmpty(cd.Trim())) continue;
                if (dic.ContainsKey(cd.Trim().ToUpper()) == false) dic.Add(cd.Trim().ToUpper(), ds);
            }

            foreach (var col in columns)
            {
                if (string.IsNullOrEmpty(col.Trim())) continue;

                string id = col;

                if (DuplicationKey.ContainsKey(id.Trim()))
                {
                    string oldid = id.Trim();
                    do
                    {
                        id = oldid + GetColName(DuplicationKey[oldid]);
                        DuplicationKey[oldid]++;

                    } while (DuplicationKey.ContainsKey(id));

                    if (DuplicationKey.ContainsKey(id.Trim()))
                    {
                        DuplicationKey[id.Trim()]++;
                    }
                    else
                    {
                        DuplicationKey.Add(id.Trim(), 0);
                    }
                }
                else DuplicationKey.Add(id.Trim(), 0);

                if (isAlignGubun)
                {
                    if (dic.ContainsKey(col.Trim().ToUpper()))
                    {
                        columnDatas.Add(id + ":" + dic[col.Trim().ToUpper()].Trim());
                    }
                    else
                    {
                        columnDatas.Add(id + ":" + col.Trim());
                    }
                }
                else
                {
                    columnDatas.Add(id);
                }
            }
            dic.Clear();
            dic = null;
            if (isAlignGubun)
            {
                return string.Join(Environment.NewLine, columnDatas.ToArray());
            }
            else
            {
                return " " + string.Join(Environment.NewLine + ",", columnDatas.ToArray());
            }
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
    }
}
