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
    /// ELEYORVOLATE.xaml 的交互逻辑
    /// </summary>
    public partial class ELEYORVOLATE : MetroWindow
    {
        public EleOrVolate SelectNum { get; set; }
        public ELEYORVOLATE()
        {
            InitializeComponent();
        }
        public ELEYORVOLATE(EleOrVolate Test)
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(new Action(delegate
            {
                if (Test == EleOrVolate.EleY)
                {
                    AlarmTextBlock.Text = "是否开始电晕实验？";
                }
                else
                {
                    AlarmTextBlock.Text = "是否开始耐压实验？";
                }
            }));
            SelectNum = Test;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (SelectNum == EleOrVolate.EleY)
                Models.AutoStateStatic.SState.IsStartEleY = true;
            else
                Models.AutoStateStatic.SState.IsStartVolate = true;
            this.Close();
        }
    }

    public enum EleOrVolate
    {
        EleY = 0,
        Volate = 1
    }
}
