using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APCUPS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public UPS UPS;
        FancyBalloon balloon;

        public MainWindow()
        {
            InitializeComponent();
            UPS = new UPS();
            balloon = new FancyBalloon(UPS.Settings);
            gbStatus.DataContext = UPS.Status;
            stackSettings.DataContext = UPS.Settings;
            UPS.Status.PropertyChanged += Status_PropertyChanged;

            cbCOMPorts.Items.Clear();
            string[] portNameCollection = SerialPort.GetPortNames();
            foreach (string port in portNameCollection)
            {
                cbCOMPorts.Items.Add(port);
            }
            cbCOMPorts.SelectedIndex = cbCOMPorts.Items.IndexOf(UPS.Settings.PortName);
            this.Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        void Status_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("PowerType"))
            {
                if (UPS.Status.PowerType == UPSStatus.PowerTypeEnum.Battery)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        balloon = new FancyBalloon(UPS.Settings);
                        MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 500000);
                    }));
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        balloon.BalloonText = "Power has been restored.";
                        balloon.BalloonTitle = "Power Restored";
                        balloon.ShowCountdown = false;
                        Timer t = new Timer();
                        t.Interval = 4000;
                        t.Elapsed += delegate
                        {
                            t.Stop();
                            MyNotifyIcon.CloseBalloon();
                        };
                        t.Start();
                    }));
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UPS.ChangeShutdownDelay((UPSStatus.GracefulDelay)Enum.Parse(typeof(UPSStatus.GracefulDelay), ((ComboBoxItem)cbShutdownDelay.SelectedItem).Content.ToString()));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UPSStatus.AlarmDelayEnum delay = new UPSStatus.AlarmDelayEnum();
            string selectedDelay = ((ComboBoxItem)cbBatteryAlarm.SelectedItem).Content.ToString().ToLower();
            switch (selectedDelay)
            {
                case "5 seconds": delay = UPSStatus.AlarmDelayEnum.FiveSeconds; break;
                case "30 seconds": delay = UPSStatus.AlarmDelayEnum.ThirtySeconds; break;
                case "low battery only": delay = UPSStatus.AlarmDelayEnum.BatteryOnly; break;
                case "disabled": delay = UPSStatus.AlarmDelayEnum.Disabled; break;
            }
            UPS.ChangeAlarmDelay(delay);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            UPS.Settings.ComputerShutdownDelay = Convert.ToInt32(txtComputerShutdownDelay.Text);
            UPSSettings.Serialize(UPS.Settings);
        }

        private void btnCOMPort_Click(object sender, RoutedEventArgs e)
        {
            UPS.Settings.PortName = cbCOMPorts.SelectedItem.ToString();
            UPSSettings.Serialize(UPS.Settings);
        }

        private void btnSendCommand_Click(object sender, RoutedEventArgs e)
        {
            var commandText = txtCommand.Text;
            txtCommandResult.Content = "";
            var commandResponse = UPS.PortManager.WriteAndWaitForResponse(commandText, 100);
            txtCommandResult.Content = commandResponse;
        }
    }
}
