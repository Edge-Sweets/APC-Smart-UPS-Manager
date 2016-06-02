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
        private UPSPortManager manager;
        public UPSSettings Settings;
        private Timer shutdownTimer;
        private Timer computerShutdownTimer;

        public UPS()
        {
            Settings = UPSSettings.Deserialize();
            manager = new UPSPortManager(Settings);
            Status = new UPSStatus(manager);
            Status.PropertyChanged += Status_PropertyChanged;
            manager.WriteAndWaitForResponse("Y", 100);
            shutdownTimer = new Timer();
            shutdownTimer.Interval = 500;
            shutdownTimer.Elapsed += t_Elapsed;
            shutdownTimer.Start();
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

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            IsClosing();
        }

        /// <summary>
        /// Method to determine if the application is closing because of a shutdown.
        /// If it is, we need to gracefully shut down the UPS as well.
        /// </summary>
        public void IsClosing()
        {
            if (computerIsShuttingDown())
            {
                ShutdownGracefully();
                shutdownTimer.Stop();
            }
        }

        private bool computerIsShuttingDown()
        {
            string query = "*[System/EventID=1074]";
            EventLogQuery elq = new EventLogQuery("System", PathType.LogName, query);
            EventLogReader elr = new EventLogReader(elq);
            EventRecord entry;
            while ((entry = elr.ReadEvent()) != null)
            {
                if (entry.TimeCreated.HasValue && entry.TimeCreated.Value.AddMinutes(1) > DateTime.Now)
                {
                    string xml = entry.ToXml();
                    if (xml.ToLower().Contains("power off"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gracefully shuts down the UPS. This is how the UPS will shut down when the computer is shutting down.
        /// </summary>
        public void ShutdownGracefully()
        {
            manager.WriteSerial("Y");
            System.Threading.Thread.Sleep(150);
            manager.WriteSerial("U");
            System.Threading.Thread.Sleep(150);
            string listen = manager.WriteAndWaitForResponse("S", 100);
            int numTries = 0;
            int maxTries = 15;
            while (listen != "OK" && numTries < maxTries)
            {
                System.Threading.Thread.Sleep(50);
                listen = manager.WriteAndWaitForResponse("S", 100);
                numTries++;
            }
            manager.WriteSerial("K");
            System.Threading.Thread.Sleep(750);
            manager.WriteSerial("K");
        }

        /// <summary>
        /// turns on the UPS if already off. Rarely, if ever, used, since the computer will be plugged into the UPS
        /// </summary>
        public void TurnOnUPS()
        {
            manager.WriteSerial(((char)14).ToString());
            System.Threading.Thread.Sleep(1500);
            manager.WriteSerial(((char)14).ToString());
        }


        
        public void ChangeShutdownDelay(APCUPS.UPSStatus.GracefulDelay newDelay)
        {
            Status.PauseTimer();
            System.Threading.Thread.Sleep(250);
            string currentDelay = manager.WriteAndWaitForResponse("p", 100);
            currentDelay = currentDelay.Replace(".", "");
            while (Convert.ToInt32(currentDelay) != (int)newDelay)
            {
                manager.WriteAndWaitForResponse("-", 50);
                currentDelay = manager.WriteAndWaitForResponse("p", 100);
                currentDelay = currentDelay.Replace(".", "");
            }
            Status.ShutdownDelay = UPSStatus.GetEnumDescription((UPSStatus.GracefulDelay)Enum.Parse(typeof(UPSStatus.GracefulDelay), currentDelay));
            Status.StartTimer();
        }

        public void ChangeAlarmDelay(UPSStatus.AlarmDelayEnum newDelay)
        {
            Status.PauseTimer();
            System.Threading.Thread.Sleep(250);
            string currentDelay = manager.WriteAndWaitForResponse("k", 100);
            currentDelay = currentDelay.Replace(".", "");
            UPSStatus.AlarmDelayEnum delayEnum = SetAlarmDelayEnum(currentDelay);

            while (delayEnum != newDelay)
            {
                manager.WriteAndWaitForResponse("-", 50);
                currentDelay = manager.WriteAndWaitForResponse("k", 100);
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
