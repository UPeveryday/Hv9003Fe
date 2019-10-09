using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Process.xaml 的交互逻辑
    /// </summary>
    public partial class Process : Window
    {
        public Process()
        {
            InitializeComponent();
            ProgressBegin();

        }

        private void ProgressBegin()
        {

            Thread thread = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    this.LaunchProgressBar.Dispatcher.BeginInvoke((ThreadStart)delegate { this.LaunchProgressBar.Value = i; });
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        IsMessage.Text = i.ToString() + " %";
                    });
                    Thread.Sleep(20);
                    if (i == 100)
                    {
                        this.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            IsMessage.Text = "100%";

                        });

                    }
                }

            }));
            thread.Start();

        }
    }
}
