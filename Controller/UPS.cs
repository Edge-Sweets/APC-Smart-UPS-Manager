using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Timers;

namespace APCUPS
{
    public class UPS
    {

        public UPSStatus Status;
        public UPSPortManager PortManager;
        public UPSSettings Settings;
        private Timer computerShutdownTimer;

        public UPS()
        {
            Settings = UPSSettings.Deserialize();
            PortManager = new UPSPortManager(Settings);
            Status = new UPSStatus(PortManager);
            Status.PropertyChanged += Status_PropertyChanged;
            PortManager.WriteAndWaitForResponse("Y", 100);
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
           
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            switch(e.Reason)
            {
                case SessionEndReasons.SystemShutdown:
                    ShutdownGracefully();
                    break;
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    ShutdownGracefully();
                    break;
            }
        }

        void Status_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PowerType"))
            {
                if (Status.PowerType == UPSStatus.PowerTypeEnum.Battery)
                {
                    computerShutdownTimer = new Timer();
                    Settings.ShutdownTimeLeft = Settings.ComputerShutdownDelay;
                    computerShutdownTimer.Interval = 1000;
                    computerShutdownTimer.Elapsed += computerShutdownTimer_Elapsed;
                    computerShutdownTimer.Start();
                }
                else
                {
                    if (computerShutdownTimer != null)
                    {
                        computerShutdownTimer.Stop();
                    }
                }
            }
        }

        void computerShutdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Settings.ShutdownTimeLeft -= 1;
            if (Settings.ShutdownTimeLeft <= 0)
            {
                ShutdownGracefully();
                Process.Start("shutdown", "/s /t 0");
            }
        }

        /// <summary>
        /// Gracefully shuts down the UPS. This is how the UPS will shut down when the computer is shutting down.
        /// </summary>
        public void ShutdownGracefully()
        {
            PortManager.WriteSerial("Y");
            System.Threading.Thread.Sleep(150);
            PortManager.WriteSerial("U");
            System.Threading.Thread.Sleep(150);
            string listen = PortManager.WriteAndWaitForResponse("S", 100);
            int numTries = 0;
            int maxTries = 15;
            while (listen != "OK" && numTries < maxTries)
            {
                System.Threading.Thread.Sleep(50);
                listen = PortManager.WriteAndWaitForResponse("S", 100);
                numTries++;
            }
            PortManager.WriteSerial("K");
            System.Threading.Thread.Sleep(750);
            PortManager.WriteSerial("K");
        }

        /// <summary>
        /// turns on the UPS if already off. Rarely, if ever, used, since the computer will be plugged into the UPS
        /// </summary>
        public void TurnOnUPS()
        {
            PortManager.WriteSerial(((char)14).ToString());
            System.Threading.Thread.Sleep(1500);
            PortManager.WriteSerial(((char)14).ToString());
        }

        public void ChangeShutdownDelay(APCUPS.UPSStatus.GracefulDelay newDelay)
        {
            Status.PauseTimer();
            System.Threading.Thread.Sleep(250);
            string currentDelay = PortManager.WriteAndWaitForResponse("p", 100);
            currentDelay = currentDelay.Replace(".", "");
            while (Convert.ToInt32(currentDelay) != (int)newDelay)
            {
                PortManager.WriteAndWaitForResponse("-", 50);
                currentDelay = PortManager.WriteAndWaitForResponse("p", 100);
                currentDelay = currentDelay.Replace(".", "");
            }
            Status.ShutdownDelay = UPSStatus.GetEnumDescription((UPSStatus.GracefulDelay)Enum.Parse(typeof(UPSStatus.GracefulDelay), currentDelay));
            Status.StartTimer();
        }

        public void ChangeAlarmDelay(UPSStatus.AlarmDelayEnum newDelay)
        {
            Status.PauseTimer();
            System.Threading.Thread.Sleep(250);
            string currentDelay = PortManager.WriteAndWaitForResponse("k", 100);
            currentDelay = currentDelay.Replace(".", "");
            UPSStatus.AlarmDelayEnum delayEnum = SetAlarmDelayEnum(currentDelay);

            while (delayEnum != newDelay)
            {
                PortManager.WriteAndWaitForResponse("-", 50);
                currentDelay = PortManager.WriteAndWaitForResponse("k", 100);
                currentDelay = currentDelay.Replace(".", "");
                delayEnum = SetAlarmDelayEnum(currentDelay);
            }
            Status.AlarmDelay = UPSStatus.GetEnumDescription(delayEnum);
            Status.StartTimer();
        }

        private static UPSStatus.AlarmDelayEnum SetAlarmDelayEnum(string currentDelay)
        {
            UPSStatus.AlarmDelayEnum delayEnum = UPSStatus.AlarmDelayEnum.Disabled;
            switch (currentDelay)
            {
                case "0": delayEnum = UPSStatus.AlarmDelayEnum.FiveSeconds; break;
                case "T": delayEnum = UPSStatus.AlarmDelayEnum.ThirtySeconds; break;
                case "L": delayEnum = UPSStatus.AlarmDelayEnum.BatteryOnly; break;
                case "N": delayEnum = UPSStatus.AlarmDelayEnum.Disabled; break;
            }
            return delayEnum;
        }

    }
}
