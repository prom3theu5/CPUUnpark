using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace UnparkCPU
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<RegData> regCollection = new List<RegData>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetRegistryData()
        {
            lblStatusText.Content = "Searching Registry ...";
            lblStatusText.Foreground = Brushes.Green;
            btnCpuStatus.IsEnabled = false;
            btnParkAll.IsEnabled = false;
            btnUnparkAll.IsEnabled = false;
            lstRegData.ItemsSource = null;

            var thread = new Thread(DoRegStuff);
            thread.Start();
        }

        private void DoRegStuff(object obj)
        {
            try
            {
                DataUtility dataUtility = new DataUtility();
                dataUtility.RegisterParserDLL();
                List<RegData> regDataCollection = null;

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if ((regDataCollection = dataUtility.SelectRegistryData()) != null)
                    {
                        regCollection = regDataCollection;
                        lstRegData.ItemsSource = regCollection;
                    }
                    lblStatusText.Content = "Ready";
                    lblStatusText.Foreground = Brushes.Black;
                    btnCpuStatus.IsEnabled = true;

                    if (regCollection != null && regCollection.Count > 0)
                    {
                        btnParkAll.IsEnabled = true;
                        btnUnparkAll.IsEnabled = true;
                    }
                }));
            }
            catch (Exception)
            {
                lblStatusText.Content = "Unexpected error";
                lblStatusText.Foreground = Brushes.Red;
                btnCpuStatus.IsEnabled = true;
            }
        }

        private void btnCpuStatus_Click(object sender, RoutedEventArgs e)
        {
            GetRegistryData();
        }

      private void btnUnparkAll_Click(object sender, RoutedEventArgs e)
        {
            if (regCollection != null && regCollection.Count > 0)
            {
                foreach (RegData data in regCollection)
                {
                    DataUtility utility = new DataUtility();
                    utility.Write(data.Path, data.ValueName, 0);
                }
                GetRegistryData();
                lblStatusText.Content = "Reboot for changes to take effect!";
                lblStatusText.Foreground = Brushes.Green;
            }
        }

        private void btnParkAll_Click(object sender, RoutedEventArgs e)
        {
            if (regCollection != null && regCollection.Count > 0)
            {
                foreach (RegData data in regCollection)
                {
                    DataUtility utility = new DataUtility();
                    utility.Write(data.Path, data.ValueName, 100);
                }
                GetRegistryData();
                lblStatusText.Content = "Reboot for changes to take effect!";
                lblStatusText.Foreground = Brushes.Green;
            }
        }
    }
}
