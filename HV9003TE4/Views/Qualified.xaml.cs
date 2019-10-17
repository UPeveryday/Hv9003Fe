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
    /// Qualified.xaml 的交互逻辑
    /// </summary>
    public partial class Qualified : MetroWindow
    {
        public Qualified()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.Quality = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.Quality = false;
            this.Close();

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
