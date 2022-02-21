using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Trader.GUI
{
    public class LogItem
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
    }

    public class LogViewer : ObservableCollection<LogItem>
    {
        public List<LogItem> items = new List<LogItem>();

        public void Filter()
        {
            Clear();
            foreach (LogItem item in items)
            {
                Add(item);
            }
        }
    }

    public partial class LogControl : UserControl
    {
        public static LogControl Instatnce;
        public LogViewer Viewer;

        public LogControl()
        {
            InitializeComponent();
            if (Instatnce == null) Instatnce = this;
            Viewer = new LogViewer();
            LogGrid.ItemsSource = Viewer;
        }

        public void AddMessage(LogItem message)
        {
            Viewer.items.Insert(0, message);
            Viewer.Filter();
            LogGrid.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Viewer.items.Clear();
            Viewer.Filter();
            LogGrid.Items.Refresh();
        }

        private void Info_Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
