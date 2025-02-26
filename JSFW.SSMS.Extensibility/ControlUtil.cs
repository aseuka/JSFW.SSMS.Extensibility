using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;

namespace JSFW.SSMS.Extensibility
{
    public static class ControlUtil
    {
        //? 이걸 어디서 받았더라...... 

        // Control拓展
        public static IEnumerable<Control> ExpandControl(this Control A_0)
        {
            IEnumerable<Control> second = A_0.Controls.Cast<Control>();
            Func<Control, IEnumerable<Control>> b = new Func<Control, IEnumerable<Control>>(filter);
            return second.SelectMany<Control, Control>(b).Concat<Control>(second);
        }


        public static TextDocument filter(Document A_0)
        {
            if (A_0 == null)
            {
                throw new Exception("document is NULL");
            }
            return (A_0.Object("TextDocument") as TextDocument);
        }

        public static TextDocument filter(Window A_0)
        {
            if (A_0 == null)
            {
                throw new Exception("window is NULL");
            }
            if (A_0.Document == null)
            {
                return null;
            }
            return (A_0.Document.Object("TextDocument") as TextDocument);
        }

        public static SqlConnection filter(UIConnectionInfo A_0)
        {
            return filter(A_0, false);
        }

        private static IEnumerable<Control> filter(Control A_0)
        {
            return A_0.ExpandControl();
        }

        public static SqlConnection filter(UIConnectionInfo A_0, bool A_1)
        {
            return new SqlConnection(buildConnection(A_0, A_1));
        }

        public static string buildConnection(UIConnectionInfo A_0, bool A_1)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = A_0.ServerName,
                IntegratedSecurity = string.IsNullOrEmpty(A_0.Password),
                Password = A_0.Password,
                UserID = A_0.UserName
            };
            if (A_1)
            {
                builder.InitialCatalog = "master";
            }
            else
            {
                builder.InitialCatalog = A_0.AdvancedOptions["DATABASE"] ?? "master";
            }
            //builder.ApplicationName = "SSMS Tools Pack";
            return builder.ToString();
        }
    }
}
