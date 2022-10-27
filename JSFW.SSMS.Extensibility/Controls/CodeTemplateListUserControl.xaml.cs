using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JSFW.Controls
{
    /// <summary>
    /// CodeTemplateListUserControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CodeTemplateListUserControl : UserControl
    {
        public CodeTemplateListUserControl()
        {
            InitializeComponent();
        }

        public event Action<string> ExportToString { add { cdTempListView.ExportToString += value; } remove { cdTempListView.ExportToString -= value; } }
        public event Func<string> ImportToString { add { cdTempListView.ImportToString += value; } remove { cdTempListView.ImportToString -= value;  } }

        internal void ListRefresh()
        {
            cdTempListView.ListRefresh();
        }
    }
}
