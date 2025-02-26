using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE80;
using EnvDTE;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
using Microsoft.SqlServer.Management.UI.Grid;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

namespace JSFW.SSMS.Extensibility
{
    /// <summary>
    /// 操作SSMS对象
    /// </summary>
    public class ShellUtilities
    {
        /// <summary>
        /// 获取当前活动文档视图窗口
        /// </summary>
        /// <param name="wf"></param>
        /// <returns></returns>
        public static Control GetDocView()
        {
            object obj;
            GetCurrentActiveFrameDocView().GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out obj);
            return obj as Control;
        }

        /// <summary>
        /// 获取当前文档视图窗口中的DataGrid
        /// </summary>
        /// <returns></returns>
        public static IGridControl GetActiveGridControl()
        {
            try
            {
                //object editCtrl = GetDocView();
                SqlScriptEditorControl sqlEditorCtl = GetDocView() as SqlScriptEditorControl;

                object sqlResultsControl = GetNonPublicField(sqlEditorCtl, "m_sqlResultsControl");
                object gridResultsPage = GetNonPublicField(sqlResultsControl, "m_gridResultsPage");

                var focusedGrid = GetNonPublicProperty(gridResultsPage, "FocusedGrid"); 
                return focusedGrid as IGridControl;
            }
            catch (Exception e)
            {
                // Log
                return null;
            }
        }
         
        /// <summary>
        /// 获取当前文档视图窗口中的DataGrid(另种方式)
        /// </summary>
        /// <returns></returns>
        public static IGridControl GetActiveGridControl2()
        {
            try
            {
                IList<Control> ctlList = GetDocView().ExpandControl().Where(s => s.GetType().FullName.Contains("GridResultsGrid")).ToList();
                if (ctlList.Count == 0)
                    return null;

                return ctlList[0] as IGridControl;
            }
            catch (Exception e)
            {
                // Log
                return null;
            }
        }

        /// <summary>
        /// 向SQL编辑器插入文本
        /// </summary>
        /// <param name="text"></param>
        public static void Insert2SqlScriptEditor(string text)
        {
            Document document = ((DTE2)ServiceCache.ExtensibilityModel).ActiveDocument;
            if (document != null)
            {
                TextSelection selection = (TextSelection)document.Selection;
                selection.Insert(text, (Int32)EnvDTE.vsInsertFlags.vsInsertFlagsContainNewText);
            }
        }


        #region 私有方法
        
        /// <summary>
        /// 获取当前活动的文档
        /// </summary>
        /// <returns></returns>
        private static IVsWindowFrame GetCurrentActiveFrameDocView()
        {
            object obj = null;
            // 第一个参数：1-当前活动窗口，2-当前活动文档
            ServiceCache.VSMonitorSelection.GetCurrentElementValue(2, ref obj);
            return obj as IVsWindowFrame;
        }

        /// <summary>
        /// 获取对象的非公开属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static object GetNonPublicField(object obj, string field)
        { 
            FieldInfo f = obj.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance );
            return f.GetValue(obj);
        }
         
        private static object GetPublicField(object obj, string field)
        {
            FieldInfo f = obj.GetType().GetField(field, BindingFlags.Public | BindingFlags.Instance);
            return f.GetValue(obj);
        }

        private static object GetPublicProperty(object obj, string propertyName)
        {
            PropertyInfo p = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            return p.GetValue(obj, null);
        }

        private static object GetNonPublicProperty(object obj, string propertyName)
        { 
            PropertyInfo p = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            return p.GetValue(obj, null);
        }

        #endregion
    }
}
