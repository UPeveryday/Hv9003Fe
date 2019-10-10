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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
namespace HV9003TE4.Views
{
    /// <summary>
    /// Alarm.xaml 的交互逻辑
    /// </summary>
    public partial class Alarm : MetroWindow
    {
        public Alarm()
        {
            InitializeComponent();
        }
        public Alarm(string Text)
        {
            InitializeComponent();
            AlarmTextBlock.Text = Text;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void grid_ms(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
