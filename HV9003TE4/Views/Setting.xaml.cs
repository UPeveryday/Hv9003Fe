﻿using System;
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
using SCEEC.Numerics;

namespace HV9003TE4.Views
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : MetroWindow
    {
        public Setting()
        {
            InitializeComponent();

        }
      

        private void Confire_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cance_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
                this.DragMove();
        }

        private void CnTanClick(object sender, RoutedEventArgs e)
        {
            Views.CnTanKeyBord keyb = new Views.CnTanKeyBord();
            keyb.OutCnTanData += Keyb_OutCnTanData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }

        private void Keyb_OutCnTanData(string cntan)
        {
            CnTanButton.Content = cntan;
            PhysicalVariable need = NumericsConverter.Text2Value(cntan);
             (this.DataContext as MainWindowModel).AGn = need;
        }

        private void Cn_CLick(object sender, RoutedEventArgs e)
        {
            Views.KeyBoard keyb = new Views.KeyBoard();
            keyb.OutCnData += Keyb_OutCnData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }

        private void Keyb_OutCnData(string cn)
        {
            CnButton.Content = cn;
            PhysicalVariable need = NumericsConverter.Text2Value(cn);
            (this.DataContext as MainWindowModel).Cn = need;
        }
    }
}